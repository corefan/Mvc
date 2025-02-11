// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc.ApiExplorer
{
    /// <summary>
    /// Represents an API exposed by this application.
    /// </summary>
    public class ApiDescription
    {
        /// <summary>
        /// Gets or sets <see cref="ActionDescriptor"/> for this api.
        /// </summary>
        public ActionDescriptor ActionDescriptor { get; set; }

        /// <summary>
        /// Gets or sets group name for this api.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the supported HTTP method for this api, or null if all HTTP methods are supported.
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// Gets a list of <see cref="ApiParameterDescription"/> for this api.
        /// </summary>
        public IList<ApiParameterDescription> ParameterDescriptions { get; } = new List<ApiParameterDescription>();

        /// <summary>
        /// Gets arbitrary metadata properties associated with the <see cref="ApiDescription"/>.
        /// </summary>
        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        /// <summary>
        /// Gets or sets relative url path template (relative to application root) for this api.
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ModelMetadata"/> for the <see cref="ResponseType"/> or null.
        /// </summary>
        /// <remarks>
        /// Will be null if <see cref="ResponseType"/> is null.
        /// </remarks>
        public ModelMetadata ResponseModelMetadata { get; set; }

        /// <summary>
        /// Gets or sets the CLR data type of the response or null.
        /// </summary>
        /// <remarks>
        /// Will be null if the action returns no response, or if the response type is unclear. Use
        /// <c>ProducesAttribute</c> on an action method to specify a response type.
        /// </remarks>
        public Type ResponseType { get; set; }

        /// <summary>
        /// Gets the list of possible formats for a response.
        /// </summary>
        /// <remarks>
        /// Will be empty if the action returns no response, or if the response type is unclear. Use
        /// <c>ProducesAttribute</c> on an action method to specify a response type.
        /// </remarks>
        public IList<ApiRequestFormat> SupportedRequestFormats { get; } = new List<ApiRequestFormat>();

        /// <summary>
        /// Gets the list of possible formats for a response.
        /// </summary>
        /// <remarks>
        /// Will be empty if the action returns no response, or if the response type is unclear. Use
        /// <c>ProducesAttribute</c> on an action method to specify a response type.
        /// </remarks>
        public IList<ApiResponseFormat> SupportedResponseFormats { get; } = new List<ApiResponseFormat>();
    }
}