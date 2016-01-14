// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.TestCommon;
using Xunit;

namespace Microsoft.AspNet.Mvc.Core
{
    /// <summary>
    /// Test the TextArea extensions in <see cref="HtmlHelperInputExtensions" /> class.
    /// </summary>
    public class HtmlHelperTextAreaExtensionsTest
    {
        [Fact]
        public void TextAreaHelpers_UsesSpecifiedExpression()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var textAreaResult = helper.TextArea("Property1");
            var textAreaForResult = helper.TextAreaFor(m => m.Property1);

            // Assert
            Assert.Equal(
                "<textarea id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\">\r\n</textarea>",
                HtmlContentUtilities.HtmlContentToString(textAreaResult));
            Assert.Equal(
                "<textarea id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\">\r\n</textarea>",
                HtmlContentUtilities.HtmlContentToString(textAreaForResult));
        }

        [Fact]
        public void TextAreaHelpers_UsesSpecifiedValue()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var textAreaResult = helper.TextArea("Property1", value: "myvalue");

            // Assert
            Assert.Equal(
                "<textarea id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\">\r\nHtmlEncode[[myvalue]]</textarea>",
                HtmlContentUtilities.HtmlContentToString(textAreaResult));
        }

        [Fact]
        public void TextAreaHelpers_UsesSpecifiedHtmlAttributes()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var textAreaResult = helper.TextArea("Property1", htmlAttributes: new { attr = "value" });
            var textAreaForResult = helper.TextAreaFor(m => m.Property1, htmlAttributes: new { attr = "value" });

            // Assert
            Assert.Equal(
                "<textarea attr=\"HtmlEncode[[value]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\">\r\n</textarea>",
                HtmlContentUtilities.HtmlContentToString(textAreaResult));
            Assert.Equal(
                "<textarea attr=\"HtmlEncode[[value]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\">\r\n</textarea>",
                HtmlContentUtilities.HtmlContentToString(textAreaForResult));
        }

        [Fact]
        public void TextAreaHelpers_UsesSpecifiedRowsAndColumns()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var textAreaResult = helper.TextArea("Property1", value: "myvalue", rows: 1, columns: 2, htmlAttributes: new { attr = "value" });
            var textAreaForResult = helper.TextAreaFor(m => m.Property1, rows: 1, columns: 2, htmlAttributes: new { attr = "value" });

            // Assert
            Assert.Equal(
                "<textarea attr=\"HtmlEncode[[value]]\" columns=\"HtmlEncode[[2]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" rows=\"HtmlEncode[[1]]\">\r\nHtmlEncode[[myvalue]]</textarea>",
                HtmlContentUtilities.HtmlContentToString(textAreaResult));
            Assert.Equal(
                "<textarea attr=\"HtmlEncode[[value]]\" columns=\"HtmlEncode[[2]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" rows=\"HtmlEncode[[1]]\">\r\n</textarea>",
                HtmlContentUtilities.HtmlContentToString(textAreaForResult));
        }
    }
}
