// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.TestCommon;
using Xunit;

namespace Microsoft.AspNet.Mvc.Core
{
    /// <summary>
    /// Test the RadioButton extensions in <see cref="HtmlHelperInputExtensions" /> class.
    /// </summary>
    public class HtmlHelperRadioButtonExtensionsTest
    {
        [Fact]
        public void RadioButtonHelpers_UsesSpecifiedExpression()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var radioButtonResult = helper.RadioButton("Property1", value: "myvalue");
            var radioButtonForResult = helper.RadioButtonFor(m => m.Property1, value: "myvalue");

            // Assert
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[radio]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(radioButtonResult));
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[radio]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(radioButtonForResult));
        }

        [Fact]
        public void RadioButtonHelpers_UsesSpecifiedIsChecked()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var radioButtonResult = helper.RadioButton("Property1", value: "myvalue", isChecked: true);

            // Assert
            Assert.Equal(
                "<input checked=\"HtmlEncode[[checked]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[radio]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(radioButtonResult));
        }

        [Fact]
        public void RadioButtonHelpers_UsesSpecifiedHtmlAttributes()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var radioButtonResult = helper.RadioButton("Property1", value: "myvalue", htmlAttributes: new { attr = "value" });
            var radioButtonForResult = helper.RadioButtonFor(m => m.Property1, value: "myvalue", htmlAttributes: new { attr = "value" });

            // Assert
            Assert.Equal(
                "<input attr=\"HtmlEncode[[value]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[radio]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(radioButtonResult));
            Assert.Equal(
                "<input attr=\"HtmlEncode[[value]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[radio]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(radioButtonForResult));
        }
    }
}
