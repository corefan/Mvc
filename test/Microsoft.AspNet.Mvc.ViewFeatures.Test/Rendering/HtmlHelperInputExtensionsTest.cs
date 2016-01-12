// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.TestCommon;
using Xunit;

namespace Microsoft.AspNet.Mvc.Core
{
    /// <summary>
    /// Test the <see cref="HtmlHelperInputExtensions" /> class.
    /// </summary>
    public class HtmlHelperInputExtensionsTest
    {
        [Fact]
        public void CheckboxHelpers_UsesSpecifiedExpression()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var checkboxResult = helper.CheckBox("Property1");

            // Assert
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[checkbox]]\" value=\"HtmlEncode[[true]]\" />" +
                "<input name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[hidden]]\" value=\"HtmlEncode[[false]]\" />",
                HtmlContentUtilities.HtmlContentToString(checkboxResult));
        }

        [Fact]
        public void CheckboxHelpers_UsesSpecifiedIsChecked()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var checkboxResult = helper.CheckBox("Property1", true);

            // Assert
            Assert.Equal(
                "<input checked=\"HtmlEncode[[checked]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[checkbox]]\" value=\"HtmlEncode[[true]]\" />" +
                "<input name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[hidden]]\" value=\"HtmlEncode[[false]]\" />",
                HtmlContentUtilities.HtmlContentToString(checkboxResult));
        }

        [Fact]
        public void CheckboxHelpers_UsesSpecifiedHtmlAttributes()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var checkboxResult = helper.CheckBox("Property1", new { attr="value" });

            // Assert
            Assert.Equal(
                "<input attr=\"HtmlEncode[[value]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[checkbox]]\" value=\"HtmlEncode[[true]]\" />" +
                "<input name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[hidden]]\" value=\"HtmlEncode[[false]]\" />",
                HtmlContentUtilities.HtmlContentToString(checkboxResult));
        }

        [Fact]
        public void HiddenHelpers_UsesSpecifiedExpression()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var hiddenResult = helper.Hidden("Property1");
            var hiddenForResult = helper.HiddenFor(m => m.Property1);

            // Assert
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[hidden]]\" value=\"\" />",
                HtmlContentUtilities.HtmlContentToString(hiddenResult));
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[hidden]]\" value=\"\" />",
                HtmlContentUtilities.HtmlContentToString(hiddenForResult));
        }

        [Fact]
        public void HiddenHelpers_UsesSpecifiedValue()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var hiddenResult = helper.Hidden("Property1", "myvalue");

            // Assert
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[hidden]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(hiddenResult));
        }

        [Fact]
        public void PasswordHelpers_UsesSpecifiedExpression()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var passwordResult = helper.Password("Property1");
            var passwordForResult = helper.PasswordFor(m => m.Property1);

            // Assert
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[password]]\" />",
                HtmlContentUtilities.HtmlContentToString(passwordResult));
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[password]]\" />",
                HtmlContentUtilities.HtmlContentToString(passwordForResult));
        }

        [Fact]
        public void PasswordHelpers_UsesSpecifiedValue()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var passwordResult = helper.Password("Property1", "myvalue");

            // Assert
            Assert.Equal(
                "<input id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[password]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(passwordResult));
        }

        [Fact]
        public void RadioButtonHelpers_UsesSpecifiedExpression()
        {
            // Arrange
            var helper = DefaultTemplatesUtilities.GetHtmlHelper();

            // Act
            var radioButtonResult = helper.RadioButton("Property1", "myvalue");
            var radioButtonForResult = helper.RadioButtonFor(m => m.Property1, "myvalue");

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
            var radioButtonResult = helper.RadioButton("Property1", "myvalue", true);

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
            var radioButtonResult = helper.RadioButton("Property1", "myvalue", new { attr="value" });
            var radioButtonForResult = helper.RadioButtonFor(m => m.Property1, "myvalue", new { attr = "value" });

            // Assert
            Assert.Equal(
                "<input attr=\"HtmlEncode[[value]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[radio]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(radioButtonResult));
            Assert.Equal(
                "<input attr=\"HtmlEncode[[value]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[radio]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(radioButtonForResult));
        }

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
            var textBoxResult = helper.TextBox("Property1", "myvalue");

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
            var textBoxResult = helper.TextBox("Property1", "myvalue", "prefix: {0}");

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
            var textBoxResult = helper.TextBox("Property1", "myvalue", new { attr="value" });
            var textBoxForResult = helper.TextBoxFor(m => m.Property1, new { attr = "value" });

            // Assert
            Assert.Equal(
                "<input attr=\"HtmlEncode[[value]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[text]]\" value=\"HtmlEncode[[myvalue]]\" />",
                HtmlContentUtilities.HtmlContentToString(textBoxResult));
            Assert.Equal(
                "<input attr=\"HtmlEncode[[value]]\" id=\"HtmlEncode[[Property1]]\" name=\"HtmlEncode[[Property1]]\" type=\"HtmlEncode[[text]]\" value=\"\" />",
                HtmlContentUtilities.HtmlContentToString(textBoxForResult));
        }

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
            var textAreaResult = helper.TextArea("Property1", "myvalue");

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
            var textAreaResult = helper.TextArea("Property1", new { attr = "value" });
            var textAreaForResult = helper.TextAreaFor(m => m.Property1, new { attr = "value" });

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
            var textAreaResult = helper.TextArea("Property1", "myvalue", 1, 2, new { attr = "value" });
            var textAreaForResult = helper.TextAreaFor(m => m.Property1, 1, 2, new { attr = "value" });

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
