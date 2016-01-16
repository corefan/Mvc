// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.ViewFeatures;

namespace Microsoft.AspNet.Mvc.ViewComponents
{
    /// <summary>
    /// A default implementation of <see cref="IViewComponentActivator"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="DefaultViewComponentActivator"/> can provide the current instance of
    /// <see cref="ViewComponentContext"/> to a public property of a view component marked
    /// with <see cref="ViewComponentContextAttribute"/>. 
    /// </remarks>
    public class DefaultViewComponentActivator : IViewComponentActivator
    {
        private readonly ITypeActivatorCache _typeActivatorCache;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultViewComponentActivator"/> class.
        /// </summary>
        public DefaultViewComponentActivator(ITypeActivatorCache typeActivatorCache)
        {
            if (typeActivatorCache == null)
            {
                throw new ArgumentNullException(nameof(typeActivatorCache));
            }

            _typeActivatorCache = typeActivatorCache;
        }

        /// <inheritdoc />
        public virtual object Create(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var componentType = context.ViewComponentDescriptor.Type.GetTypeInfo();

            if (!componentType.IsClass ||
                !componentType.IsPublic ||
                componentType.IsAbstract ||
                componentType.ContainsGenericParameters)
            {
                var message = Resources.FormatValueInterfaceAbstractOrOpenGenericTypesCannotBeActivated(
                    componentType.FullName,
                    GetType().FullName);

                throw new InvalidOperationException(message);
            }

            var viewComponent = _typeActivatorCache.CreateInstance<object>(
            context.ViewContext.HttpContext.RequestServices,
            context.ViewComponentDescriptor.Type);

            return viewComponent;
        }

        /// <inheritdoc />
        public virtual void Release(ViewComponentContext context, object viewComponent)
        {
            if (context == null)
            {
                throw new InvalidOperationException(nameof(context));
            }

            if (viewComponent == null)
            {
                throw new InvalidOperationException(nameof(viewComponent));
            }

            var disposable = viewComponent as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}