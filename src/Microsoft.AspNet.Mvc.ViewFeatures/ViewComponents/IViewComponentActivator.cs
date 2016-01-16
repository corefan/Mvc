// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Mvc.ViewComponents
{
    /// <summary>
    /// Provides methods to activate an instantiated ViewComponent
    /// </summary>
    public interface IViewComponentActivator
    {
        /// <summary>
        /// Instantiates and activates a ViewComponent.
        /// </summary>
        /// <param name="context">
        /// The <see cref="ViewComponentContext"/> for the executing <see cref="ViewComponent"/>.
        /// </param>
        object Create(ViewComponentContext context);

        /// <summary>
        /// Releases a ViewComponent instance.
        /// </summary>
        /// <param name="viewComponent">The <see cref="ViewComponent"/> to release.</param>
        void Release(ViewComponentContext context, object viewComponent);
    }
}