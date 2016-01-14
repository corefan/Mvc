// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.TestCommon;
using Xunit;

namespace Microsoft.AspNet.Mvc.Core
{
    /// <summary>
    /// Test the TextBox extensions in <see cref="HtmlHelperInputExtensions" /> class.
    /// </summary>
    public class HtmlHelperTextBoxExtensionsTest
    {
        [Fact]
        public void TextBoxHelpers_UsesSpecifiedExpression()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var textBoxResult = helper.TextBox("Property1");
            var textBoxForResult = helper.TextBoxFor(m => m.Property1);

            // Assert
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[text]]\" value=\"\" />",
                HtmlContentUtilities.HtmlContentToString(textBoxResult));
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[text]]\" value=\"\" />",
                HtmlContentUtilities.HtmlContentToString(textBoxForResult));
        }

        [Fact]
        public void TextBoxHelpers_UsesSpecifiedValue()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var textBoxResult = helper.TextBox("Property1", value: "myvalue");

            // Assert
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[text]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(textBoxResult));
        }

        [Fact]
        public void TextBoxHelpers_UsesSpecifiedFormat()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var textBoxResult = helper.TextBox("Property1", value: "myvalue", format: "prefix: {0}");

            // Assert
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[text]]\" value=\"HtmlEncode[[prefix: myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(textBoxResult));
        }

        [Fact]
        public void TextBoxHelpers_UsesSpecifiedHtmlAttributes()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var textBoxResult = helper.TextBox("Property1", value: "myvalue", htmlAttributes: new { attr = "value" });
            var textBoxForResult = helper.TextBoxFor(m => m.Property1, htmlAttributes: new { attr = "value" });

            // Assert
            Assert.Equal(
                "<input attr=\"HtmlEncode[[value]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[text]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(textBoxResult));
            Assert.Equal(
                "<input attr=\"HtmlEncode[[value]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[text]]\" value=\"\" />",
                HtmlContentUtilities.HtmlContentToString(textBoxForResult));
        }
    }
}
