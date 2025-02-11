// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Formatters.Internal;
using Microsoft.AspNet.Mvc.Formatters.Json.Internal;
using Microsoft.AspNet.Mvc.Formatters.Json.Logging;
using Microsoft.AspNet.Mvc.Internal;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;

namespace Microsoft.AspNet.Mvc.Formatters
{
    /// <summary>
    /// An <see cref="InputFormatter"/> for JSON content.
    /// </summary>
    public class JsonInputFormatter : InputFormatter
    {
        private readonly IArrayPool<char> _charPool;
        private readonly ILogger _logger;
        private readonly ObjectPoolProvider _objectPoolProvider;
        private ObjectPool<JsonSerializer> _jsonSerializerPool;
        private JsonSerializerSettings _serializerSettings;

        /// <summary>
        /// Initializes a new instance of <see cref="JsonInputFormatter"/>.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public JsonInputFormatter(ILogger logger)
            : this(logger, SerializerSettingsProvider.CreateSerializerSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonInputFormatter"/>.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/>.</param>
        public JsonInputFormatter(ILogger logger, JsonSerializerSettings serializerSettings)
            : this(
                  logger,
                  serializerSettings,
                  ArrayPool<char>.Shared,
                  new DefaultObjectPoolProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonInputFormatter"/>.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="charPool">The <see cref="ArrayPool{char}"/>.</param>
        /// <param name="objectPoolProvider">The <see cref="ObjectPoolProvider"/>.</param>
        public JsonInputFormatter(
            ILogger logger,
            JsonSerializerSettings serializerSettings,
            ArrayPool<char> charPool,
            ObjectPoolProvider objectPoolProvider)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (serializerSettings == null)
            {
                throw new ArgumentNullException(nameof(serializerSettings));
            }

            if (charPool == null)
            {
                throw new ArgumentNullException(nameof(charPool));
            }

            if (objectPoolProvider == null)
            {
                throw new ArgumentNullException(nameof(objectPoolProvider));
            }

            _logger = logger;
            _serializerSettings = serializerSettings;
            _charPool = new JsonArrayPool<char>(charPool);
            _objectPoolProvider = objectPoolProvider;

            SupportedEncodings.Add(UTF8EncodingWithoutBOM);
            SupportedEncodings.Add(UTF16EncodingLittleEndian);

            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationJson);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.TextJson);
        }

        /// <summary>
        /// Gets or sets the <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.
        /// </summary>
        /// <remarks>
        /// Any modifications to the <see cref="JsonSerializerSettings"/> object after this
        /// <see cref="JsonInputFormatter"/> has been used will have no effect.
        /// </remarks>
        public JsonSerializerSettings SerializerSettings
        {
            get
            {
                return _serializerSettings;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _serializerSettings = value;
            }
        }

        /// <inheritdoc />
        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Get the character encoding for the content.
            var effectiveEncoding = SelectCharacterEncoding(context);
            if (effectiveEncoding == null)
            {
                return InputFormatterResult.FailureAsync();
            }

            var request = context.HttpContext.Request;
            using (var streamReader = context.ReaderFactory(request.Body, effectiveEncoding))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    jsonReader.ArrayPool = _charPool;
                    jsonReader.CloseInput = false;

                    var successful = true;
                    EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> errorHandler = (sender, eventArgs) =>
                    {
                        successful = false;

                        var exception = eventArgs.ErrorContext.Error;

                        // Handle path combinations such as "" + "Property", "Parent" + "Property", or "Parent" + "[12]".
                        var key = eventArgs.ErrorContext.Path;
                        if (!string.IsNullOrEmpty(context.ModelName))
                        {
                            if (string.IsNullOrEmpty(eventArgs.ErrorContext.Path))
                            {
                                key = context.ModelName;
                            }
                            else if (eventArgs.ErrorContext.Path[0] == '[')
                            {
                                key = context.ModelName + eventArgs.ErrorContext.Path;
                            }
                            else
                            {
                                key = context.ModelName + "." + eventArgs.ErrorContext.Path;
                            }
                        }

                        var metadata = GetPathMetadata(context.Metadata, eventArgs.ErrorContext.Path);
                        context.ModelState.TryAddModelError(key, eventArgs.ErrorContext.Error, metadata);

                        _logger.JsonInputException(eventArgs.ErrorContext.Error);

                        // Error must always be marked as handled
                        // Failure to do so can cause the exception to be rethrown at every recursive level and
                        // overflow the stack for x64 CLR processes
                        eventArgs.ErrorContext.Handled = true;
                    };

                    var type = context.ModelType;
                    var jsonSerializer = CreateJsonSerializer();
                    jsonSerializer.Error += errorHandler;
                    object model;
                    try
                    {
                        model = jsonSerializer.Deserialize(jsonReader, type);
                    }
                    finally
                    {
                        // Clean up the error handler since CreateJsonSerializer() pools instances.
                        jsonSerializer.Error -= errorHandler;
                        ReleaseJsonSerializer(jsonSerializer);
                    }

                    if (successful)
                    {
                        return InputFormatterResult.SuccessAsync(model);
                    }

                    return InputFormatterResult.FailureAsync();
                }
            }
        }

        /// <summary>
        /// Called during deserialization to get the <see cref="JsonSerializer"/>.
        /// </summary>
        /// <returns>The <see cref="JsonSerializer"/> used during deserialization.</returns>
        /// <remarks>
        /// This method works in tandem with <see cref="ReleaseJsonSerializer(JsonSerializer)"/> to
        /// manage the lifetimes of <see cref="JsonSerializer"/> instances.
        /// </remarks>
        protected virtual JsonSerializer CreateJsonSerializer()
        {
            if (_jsonSerializerPool == null)
            {
                _jsonSerializerPool = _objectPoolProvider.Create<JsonSerializer>();
            }

            return _jsonSerializerPool.Get();
        }

        /// <summary>
        /// Releases the <paramref name="serializer"/> instance.
        /// </summary>
        /// <param name="serializer">The <see cref="JsonSerializer"/> to release.</param>
        /// <remarks>
        /// This method works in tandem with <see cref="ReleaseJsonSerializer(JsonSerializer)"/> to
        /// manage the lifetimes of <see cref="JsonSerializer"/> instances.
        /// </remarks>
        protected virtual void ReleaseJsonSerializer(JsonSerializer serializer)
            => _jsonSerializerPool.Return(serializer);

        private ModelMetadata GetPathMetadata(ModelMetadata metadata, string path)
        {
            var index = 0;
            while (index >= 0 && index < path.Length)
            {
                if (path[index] == '[')
                {
                    // At start of "[0]".
                    if (metadata.ElementMetadata == null)
                    {
                        // Odd case but don't throw just because ErrorContext had an odd-looking path.
                        break;
                    }

                    metadata = metadata.ElementMetadata;
                    index = path.IndexOf(']', index);
                }
                else if (path[index] == '.' || path[index] == ']')
                {
                    // Skip '.' in "prefix.property" or "[0].property" or ']' in "[0]".
                    index++;
                }
                else
                {
                    // At start of "property", "property." or "property[0]".
                    var endIndex = path.IndexOfAny(new[] { '.', '[' }, index);
                    if (endIndex == -1)
                    {
                        endIndex = path.Length;
                    }

                    var propertyName = path.Substring(index, endIndex - index);
                    if (metadata.Properties[propertyName] == null)
                    {
                        // Odd case but don't throw just because ErrorContext had an odd-looking path.
                        break;
                    }

                    metadata = metadata.Properties[propertyName];
                    index = endIndex;
                }
            }

            return metadata;
        }
    }
}
