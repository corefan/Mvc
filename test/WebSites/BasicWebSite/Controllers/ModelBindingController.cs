// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;

namespace BasicWebSite.Controllers
{
    public class ModelBindingController : Controller
    {
        [HttpPost]
        public IActionResult Post([FromBody]int data)
        {
            return Ok(data);
        }
    }
}
