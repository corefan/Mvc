// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class ModelBindingTests : IClassFixture<MvcTestFixture<BasicWebSite.Startup>>
    {
        public ModelBindingTests(MvcTestFixture<BasicWebSite.Startup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task SendingPayloadWithNotSupportedContentType_ReturnsStatus415UnsupportedMediaType()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/ModelBinding/Post");
            request.Content = new StringContent("5", Encoding.UTF8, "invalid/json");

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        }
    }
}
