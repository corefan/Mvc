// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
#if DNXCORE50
using System.Reflection;
#endif
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding.Metadata;
using Microsoft.AspNet.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc.ModelBinding.Validation
{
    public class DefaultObjectValidatorTests
    {
        private IModelMetadataProvider MetadataProvider { get; } = TestModelMetadataProvider.CreateDefaultProvider();

        [Fact]
        public void Validate_SimpleValueType_Valid_WithPrefix()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)15;

            modelState.SetModelValue("parameter", "15", "15");
            validationState.Add(model, new ValidationStateEntry() { Key = "parameter" });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "parameter", model);

            // Assert
            AssertKeysEqual(modelState, "parameter");

            var entry = modelState["parameter"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }

        [Fact]
        public void Validate_SimpleReferenceType_Valid_WithPrefix()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)"test";

            modelState.SetModelValue("parameter", "test", "test");
            validationState.Add(model, new ValidationStateEntry() { Key = "parameter" });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "parameter", model);

            // Assert
            Assert.True(modelState.IsValid);
            AssertKeysEqual(modelState, "parameter");

            var entry = modelState["parameter"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }

        [Fact]
        public void Validate_SimpleType_MaxErrorsReached()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)"test";

            modelState.MaxAllowedErrors = 1;
            modelState.AddModelError("other.Model", "error");
            modelState.SetModelValue("parameter", "test", "test");
            validationState.Add(model, new ValidationStateEntry() { Key = "parameter" });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "parameter", model);

            // Assert
            Assert.False(modelState.IsValid);
            AssertKeysEqual(modelState, string.Empty, "parameter");

            var entry = modelState["parameter"];
            Assert.Equal(ModelValidationState.Skipped, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }

        [Fact]
        public void Validate_SimpleType_SuppressValidation()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)"test";

            modelState.SetModelValue("parameter", "test", "test");
            validationState.Add(model, new ValidationStateEntry() { Key = "parameter", SuppressValidation = true });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "parameter", model);

            // Assert
            Assert.True(modelState.IsValid);
            AssertKeysEqual(modelState, "parameter");

            var entry = modelState["parameter"];
            Assert.Equal(ModelValidationState.Skipped, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }


        [Fact]
        public void Validate_ComplexValueType_Valid()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new ValueType() { Reference = "ref", Value = 256 };

            modelState.SetModelValue("parameter.Reference", "ref", "ref");
            modelState.SetModelValue("parameter.Value", "256", "256");
            validationState.Add(model, new ValidationStateEntry() { Key = "parameter" });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "parameter", model);

            // Assert
            Assert.True(modelState.IsValid);
            AssertKeysEqual(modelState, "parameter.Reference", "parameter.Value");

            var entry = modelState["parameter.Reference"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);

            entry = modelState["parameter.Value"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }

        [Fact]
        public void Validate_ComplexReferenceType_Valid()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new ReferenceType() { Reference = "ref", Value = 256 };

            modelState.SetModelValue("parameter.Reference", "ref", "ref");
            modelState.SetModelValue("parameter.Value", "256", "256");
            validationState.Add(model, new ValidationStateEntry() { Key = "parameter" });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "parameter", model);

            // Assert
            Assert.True(modelState.IsValid);
            AssertKeysEqual(modelState, "parameter.Reference", "parameter.Value");

            var entry = modelState["parameter.Reference"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);

            entry = modelState["parameter.Value"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }

        [Fact]
        public void Validate_ComplexReferenceType_Invalid()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new Person();

            validationState.Add(model, new ValidationStateEntry() { Key = string.Empty });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, string.Empty, model);

            // Assert
            Assert.False(modelState.IsValid);
            AssertKeysEqual(modelState, "Name", "Profession");

            var entry = modelState["Name"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            var error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Name"), error.ErrorMessage);

            entry = modelState["Profession"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Profession"), error.ErrorMessage);
        }

        [Fact]
        public void Validate_ComplexType_SuppressValidation()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = new Person2()
            {
                Name = "Billy",
                Address = new Address { Street = "GreaterThan5Characters" }
            };

            modelState.SetModelValue("person.Name", "Billy", "Billy");
            modelState.SetModelValue("person.Address.Street", "GreaterThan5Characters", "GreaterThan5Characters");
            validationState.Add(model, new ValidationStateEntry() { Key = "person" });
            validationState.Add(model.Address, new ValidationStateEntry()
            {
                Key = "person.Address",
                SuppressValidation = true
            });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "person", model);

            // Assert
            Assert.True(modelState.IsValid);
            AssertKeysEqual(modelState, "person.Name", "person.Address.Street");

            var entry = modelState["person.Name"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);

            entry = modelState["person.Address.Street"];
            Assert.Equal(ModelValidationState.Skipped, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }

        [Fact]
        [ReplaceCulture]
        public void Validate_ComplexReferenceType_Invalid_MultipleErrorsOnProperty()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new Address() { Street = "Microsoft Way" };

            modelState.SetModelValue("parameter.Street", "Microsoft Way", "Microsoft Way");
            validationState.Add(model, new ValidationStateEntry() { Key = "parameter" });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "parameter", model);

            // Assert
            Assert.False(modelState.IsValid);
            AssertKeysEqual(modelState, "parameter.Street");

            var entry = modelState["parameter.Street"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);

            Assert.Equal(2, entry.Errors.Count);
            var errorMessages = entry.Errors.Select(e => e.ErrorMessage);
            Assert.Contains(ValidationAttributeUtil.GetStringLengthErrorMessage(null, 5, "Street"), errorMessages);
            Assert.Contains(ValidationAttributeUtil.GetRegExErrorMessage("hehehe", "Street"), errorMessages);
        }

        [Fact]
        [ReplaceCulture]
        public void Validate_ComplexReferenceType_Invalid_MultipleErrorsOnProperty_EmptyPrefix()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new Address() { Street = "Microsoft Way" };

            modelState.SetModelValue("Street", "Microsoft Way", "Microsoft Way");
            validationState.Add(model, new ValidationStateEntry() { Key = string.Empty });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, string.Empty, model);

            // Assert
            Assert.False(modelState.IsValid);
            AssertKeysEqual(modelState, "Street");

            var entry = modelState["Street"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);

            Assert.Equal(2, entry.Errors.Count);
            var errorMessages = entry.Errors.Select(e => e.ErrorMessage);
            Assert.Contains(ValidationAttributeUtil.GetStringLengthErrorMessage(null, 5, "Street"), errorMessages);
            Assert.Contains(ValidationAttributeUtil.GetRegExErrorMessage("hehehe", "Street"), errorMessages);
        }

        [Fact]
        [ReplaceCulture]
        public void Validate_NestedComplexReferenceType_Invalid()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new Person() { Name = "Rick", Friend = new Person() };

            modelState.SetModelValue("Name", "Rick", "Rick");
            validationState.Add(model, new ValidationStateEntry() { Key = string.Empty });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, string.Empty, model);

            // Assert
            Assert.False(modelState.IsValid);
            AssertKeysEqual(modelState, "Name", "Profession", "Friend.Name", "Friend.Profession");

            var entry = modelState["Name"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);

            entry = modelState["Profession"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            var error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Profession"), error.ErrorMessage);

            entry = modelState["Friend.Name"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Name"), error.ErrorMessage);

            entry = modelState["Friend.Profession"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Profession"), error.ErrorMessage);
        }

        // IValidatableObject is significant because the validators are on the object
        // itself, not just the properties.
        [Fact]
        [ReplaceCulture]
        public void Validate_ComplexType_IValidatableObject_Invalid()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new ValidatableModel();

            modelState.SetModelValue("parameter", "model", "model");

            validationState.Add(model, new ValidationStateEntry() { Key = "parameter" });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "parameter", model);

            // Assert
            Assert.False(modelState.IsValid);
            AssertKeysEqual(modelState, "parameter", "parameter.Property1", "parameter.Property2", "parameter.Property3");

            var entry = modelState["parameter"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            var error = Assert.Single(entry.Errors);
            Assert.Equal("Error1", error.ErrorMessage);

            entry = modelState["parameter.Property1"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal("Error2", error.ErrorMessage);

            entry = modelState["parameter.Property2"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal("Error3", error.ErrorMessage);

            entry = modelState["parameter.Property3"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal("Error3", error.ErrorMessage);
        }

        [Fact]
        public void Validate_ComplexType_IValidatableObject_CanUseRequestServices()
        {
            // Arrange
            var service = new Mock<IExampleService>();
            service.Setup(x => x.DoSomething()).Verifiable();

            var provider = new ServiceCollection().AddSingleton(service.Object).BuildServiceProvider();

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(x => x.RequestServices).Returns(provider);

            var actionContext = new ActionContext { HttpContext = httpContext.Object };

            var validatorProvider = CreateValidatorProvider();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = new Mock<IValidatableObject>();
            model
                .Setup(x => x.Validate(It.IsAny<ValidationContext>()))
                .Callback((ValidationContext context) =>
                {
                    var receivedService = context.GetService<IExampleService>();
                    Assert.Equal(service.Object, receivedService);
                    receivedService.DoSomething();
                })
                .Returns(new List<ValidationResult>());

            // Act
            validator.Validate(
                actionContext, 
                validatorProvider, 
                validationState, 
                null, 
                model.Object);

            // Assert
            service.Verify();
        }

        [Fact]
        [ReplaceCulture]
        public void Validate_ComplexType_FieldsAreIgnored_Valid()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new VariableTest() { test = 5 };

            modelState.SetModelValue("parameter", "5", "5");
            validationState.Add(model, new ValidationStateEntry() { Key = "parameter" });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "parameter", model);

            // Assert
            Assert.True(modelState.IsValid);
            Assert.Equal(1, modelState.Count);

            var entry = modelState["parameter"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }

        [Fact]
        [ReplaceCulture]
        public void Validate_ComplexType_CyclesNotFollowed_Invalid()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var person = new Person() { Name = "Billy" };
            person.Friend = person;

            var model = (object)person;

            modelState.SetModelValue("parameter.Name", "Billy", "Billy");
            validationState.Add(model, new ValidationStateEntry() { Key = "parameter" });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "parameter", model);

            // Assert
            Assert.False(modelState.IsValid);
            AssertKeysEqual(modelState, "parameter.Name", "parameter.Profession");

            var entry = modelState["parameter.Name"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);

            entry = modelState["parameter.Profession"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            var error = Assert.Single(entry.Errors);
            Assert.Equal(error.ErrorMessage, ValidationAttributeUtil.GetRequiredErrorMessage("Profession"));
        }

        [Fact]
        public void Validate_ComplexType_ShortCircuit_WhenMaxErrorCountIsSet()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator(typeof(string));

            var model = new User()
            {
                Password = "password-val",
                ConfirmPassword = "not-password-val"
            };

            modelState.MaxAllowedErrors = 2;
            modelState.AddModelError("key1", "error1");
            modelState.SetModelValue("user.Password", "password-val", "password-val");
            modelState.SetModelValue("user.ConfirmPassword", "not-password-val", "not-password-val");

            validationState.Add(model, new ValidationStateEntry() { Key = "user", });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "user", model);

            // Assert
            Assert.False(modelState.IsValid);
            AssertKeysEqual(modelState, string.Empty, "key1", "user.ConfirmPassword", "user.Password");

            var entry = modelState[string.Empty];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            var error = Assert.Single(entry.Errors);
            Assert.IsType<TooManyModelErrorsException>(error.Exception);
        }

        [Fact]
        [ReplaceCulture]
        public void Validate_CollectionType_ArrayOfSimpleType_Valid_DefaultKeyPattern()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new int[] { 5, 17 };

            modelState.SetModelValue("parameter[0]", "5", "17");
            modelState.SetModelValue("parameter[1]", "17", "5");
            validationState.Add(model, new ValidationStateEntry() { Key = "parameter" });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "parameter", model);

            // Assert
            Assert.True(modelState.IsValid);
            AssertKeysEqual(modelState, "parameter[0]", "parameter[1]");

            var entry = modelState["parameter[0]"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);

            entry = modelState["parameter[0]"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }

        [Fact]
        [ReplaceCulture]
        public void Validate_CollectionType_ArrayOfComplexType_Invalid()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new Person[] { new Person(), new Person() };

            validationState.Add(model, new ValidationStateEntry() { Key = string.Empty });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, string.Empty, model);

            // Assert
            Assert.False(modelState.IsValid);
            AssertKeysEqual(modelState, "[0].Name", "[0].Profession", "[1].Name", "[1].Profession");

            var entry = modelState["[0].Name"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            var error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Name"), error.ErrorMessage);

            entry = modelState["[0].Profession"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Profession"), error.ErrorMessage);

            entry = modelState["[1].Name"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Name"), error.ErrorMessage);

            entry = modelState["[1].Profession"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Profession"), error.ErrorMessage);
        }

        [Fact]
        [ReplaceCulture]
        public void Validate_CollectionType_ListOfComplexType_Invalid()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new List<Person> { new Person(), new Person() };

            validationState.Add(model, new ValidationStateEntry() { Key = string.Empty });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, string.Empty, model);

            // Assert
            Assert.False(modelState.IsValid);
            AssertKeysEqual(modelState, "[0].Name", "[0].Profession", "[1].Name", "[1].Profession");

            var entry = modelState["[0].Name"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            var error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Name"), error.ErrorMessage);

            entry = modelState["[0].Profession"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Profession"), error.ErrorMessage);

            entry = modelState["[1].Name"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Name"), error.ErrorMessage);

            entry = modelState["[1].Profession"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(ValidationAttributeUtil.GetRequiredErrorMessage("Profession"), error.ErrorMessage);
        }

        public static TheoryData<object, Type> ValidCollectionData
        {
            get
            {
                return new TheoryData<object, Type>()
                {
                    { new int[] { 1, 2, 3 }, typeof(int[]) },
                    { new string[] { "Foo", "Bar", "Baz" }, typeof(string[]) },
                    { new List<string> { "Foo", "Bar", "Baz" }, typeof(IList<string>)},
                    { new HashSet<string> { "Foo", "Bar", "Baz" }, typeof(string[]) },
                    {
                        new List<DateTime>
                        {
                            DateTime.Parse("1/1/14"),
                            DateTime.Parse("2/1/14"),
                            DateTime.Parse("3/1/14"),
                        },
                        typeof(ICollection<DateTime>)
                    },
                    {
                        new HashSet<Uri>
                        {
                            new Uri("http://example.com/1"),
                            new Uri("http://example.com/2"),
                            new Uri("http://example.com/3"),
                        },
                        typeof(HashSet<Uri>)
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionData))]
        public void Validate_IndexedCollectionTypes_Valid(object model, Type type)
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            modelState.Add("items[0]", new ModelStateEntry());
            modelState.Add("items[1]", new ModelStateEntry());
            modelState.Add("items[2]", new ModelStateEntry());
            validationState.Add(model, new ValidationStateEntry()
            {
                Key = "items",
                
                // Force the validator to treat it as the specified type.
                Metadata = MetadataProvider.GetMetadataForType(type),
            });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "items", model);

            // Assert
            Assert.True(modelState.IsValid);
            AssertKeysEqual(modelState, "items[0]", "items[1]", "items[2]");

            var entry = modelState["items[0]"];
            Assert.Equal(entry.ValidationState, ModelValidationState.Valid);
            Assert.Empty(entry.Errors);

            entry = modelState["items[1]"];
            Assert.Equal(entry.ValidationState, ModelValidationState.Valid);
            Assert.Empty(entry.Errors);

            entry = modelState["items[2]"];
            Assert.Equal(entry.ValidationState, ModelValidationState.Valid);
            Assert.Empty(entry.Errors);
        }

        [Fact]
        public void Validate_CollectionType_DictionaryOfSimpleType_Invalid()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = new Dictionary<string, string>()
            {
                { "FooKey", "FooValue" },
                { "BarKey", "BarValue" }
            };

            modelState.Add("items[0].Key", new ModelStateEntry());
            modelState.Add("items[0].Value", new ModelStateEntry());
            modelState.Add("items[1].Key", new ModelStateEntry());
            modelState.Add("items[1].Value", new ModelStateEntry());
            validationState.Add(model, new ValidationStateEntry() { Key = "items" });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "items", model);

            // Assert
            Assert.True(modelState.IsValid);
            AssertKeysEqual(modelState, "items[0].Key", "items[0].Value", "items[1].Key", "items[1].Value");

            var entry = modelState["items[0].Key"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);

            entry = modelState["items[0].Value"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);

            entry = modelState["items[1].Key"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);

            entry = modelState["items[1].Value"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }

        [Fact]
        [ReplaceCulture]
        public void Validate_CollectionType_DictionaryOfComplexType_Invalid()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = (object)new Dictionary<string, Person> { { "Joe", new Person() }, { "Mark", new Person() } };

            modelState.SetModelValue("[0].Key", "Joe", "Joe");
            modelState.SetModelValue("[1].Key", "Mark", "Mark");
            validationState.Add(model, new ValidationStateEntry() { Key = string.Empty });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, string.Empty, model);

            // Assert
            Assert.False(modelState.IsValid);
            AssertKeysEqual(
                modelState,
                "[0].Key",
                "[0].Value.Name", 
                "[0].Value.Profession",
                "[1].Key",
                "[1].Value.Name",
                "[1].Value.Profession");

            var entry = modelState["[0].Key"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);

            entry = modelState["[1].Key"];
            Assert.Equal(ModelValidationState.Valid, entry.ValidationState);
            Assert.Empty(entry.Errors);

            entry = modelState["[0].Value.Name"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            var error = Assert.Single(entry.Errors);
            Assert.Equal(error.ErrorMessage, ValidationAttributeUtil.GetRequiredErrorMessage("Name"));

            entry = modelState["[0].Value.Profession"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(error.ErrorMessage, ValidationAttributeUtil.GetRequiredErrorMessage("Profession"));

            entry = modelState["[1].Value.Name"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(error.ErrorMessage, ValidationAttributeUtil.GetRequiredErrorMessage("Name"));

            entry = modelState["[1].Value.Profession"];
            Assert.Equal(ModelValidationState.Invalid, entry.ValidationState);
            error = Assert.Single(entry.Errors);
            Assert.Equal(error.ErrorMessage, ValidationAttributeUtil.GetRequiredErrorMessage("Profession"));
        }

        [Fact]
        [ReplaceCulture]
        public void Validate_DoesntCatchExceptions_FromPropertyAccessors()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = new ThrowingProperty();

            // Act & Assert
            Assert.Throws(
                typeof(InvalidTimeZoneException),
                () =>
                {
                    validator.Validate(actionContext, validatorProvider, validationState, string.Empty, model);
                });
        }

        // We use the reference equality comparer for breaking cycles
        [Fact]
        public void Validate_DoesNotUseOverridden_GetHashCodeOrEquals()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator();

            var model = new TypeThatOverridesEquals[]
            {
                new TypeThatOverridesEquals { Funny = "hehe" },
                new TypeThatOverridesEquals { Funny = "hehe" }
            };

            // Act & Assert (does not throw)
            validator.Validate(actionContext, validatorProvider, validationState, string.Empty, model);
        }

        [Fact]
        public void Validate_ForExcludedComplexType_PropertiesMarkedAsSkipped()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator(typeof(User));

            var model = new User()
            {
                Password = "password-val",
                ConfirmPassword = "not-password-val"
            };

            // Note that user.ConfirmPassword has no entry in modelstate - we should not
            // create one just to mark it as skipped.
            modelState.SetModelValue("user.Password", "password-val", "password-val");
            validationState.Add(model, new ValidationStateEntry() { Key = "user", });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "user", model);

            // Assert
            Assert.Equal(ModelValidationState.Valid, modelState.ValidationState);
            AssertKeysEqual(modelState, "user.Password");

            var entry = modelState["user.Password"];
            Assert.Equal(ModelValidationState.Skipped, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }

        [Fact]
        public void Validate_ForExcludedCollectionType_PropertiesMarkedAsSkipped()
        {
            // Arrange
            var validatorProvider = CreateValidatorProvider();
            var actionContext = new ActionContext();
            var modelState = actionContext.ModelState;
            var validationState = new ValidationStateDictionary();

            var validator = CreateValidator(typeof(List<string>));

            var model = new List<string>()
            {
                "15",
            };

            modelState.SetModelValue("userIds[0]", "15", "15");
            validationState.Add(model, new ValidationStateEntry() { Key = "userIds", });

            // Act
            validator.Validate(actionContext, validatorProvider, validationState, "userIds", model);

            // Assert
            Assert.Equal(ModelValidationState.Valid, modelState.ValidationState);
            AssertKeysEqual(modelState, "userIds[0]");

            var entry = modelState["userIds[0]"];
            Assert.Equal(ModelValidationState.Skipped, entry.ValidationState);
            Assert.Empty(entry.Errors);
        }

        private static IModelValidatorProvider CreateValidatorProvider()
        {
            return TestModelValidatorProvider.CreateDefaultProvider();
        }

        private static DefaultObjectValidator CreateValidator(Type excludedType)
        {
            var excludeFilters = new List<ValidationExcludeFilter>();
            if (excludedType != null)
            {
                excludeFilters.Add(new ValidationExcludeFilter(excludedType));
            }

            var provider = TestModelMetadataProvider.CreateDefaultProvider(excludeFilters.ToArray());
            return new DefaultObjectValidator(provider);
        }

        private static DefaultObjectValidator CreateValidator(params IMetadataDetailsProvider[] providers)
        {
            var provider = TestModelMetadataProvider.CreateDefaultProvider(providers);
            return new DefaultObjectValidator(provider);
        }

        private static void AssertKeysEqual(ModelStateDictionary modelState, params string[] keys)
        {
            Assert.Equal<string>(keys.OrderBy(k => k).ToArray(), modelState.Keys.OrderBy(k => k).ToArray());
        }

        private class ThrowingProperty
        {
            public string WatchOut
            {
                get
                {
                    throw new InvalidTimeZoneException();
                }
            }
        }

        private class Person
        {
            [Required, StringLength(10)]
            public string Name { get; set; }

            [Required]
            public string Profession { get; set; }

            public Person Friend { get; set; }
        }

        private class Person2
        {
            public string Name { get; set; }
            public Address Address { get; set; }
        }

        private class Address
        {
            [StringLength(5)]
            [RegularExpression("hehehe")]
            public string Street { get; set; }
        }

        private struct ValueType
        {
            public int Value { get; set; }
            public string Reference { get; set; }
        }

        private class ReferenceType
        {
            public int Value { get; set; }
            public string Reference { get; set; }
        }

        private class ValidatableModel : IValidatableObject
        {
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                yield return new ValidationResult("Error1", new string[] { });
                yield return new ValidationResult("Error2", new[] { "Property1" });
                yield return new ValidationResult("Error3", new[] { "Property2", "Property3" });
            }
        }

        private class TypeThatOverridesEquals
        {
            [StringLength(2)]
            public string Funny { get; set; }

            public override bool Equals(object obj)
            {
                throw new InvalidOperationException();
            }

            public override int GetHashCode()
            {
                throw new InvalidOperationException();
            }
        }

        private class VariableTest
        {
            [Range(15, 25)]
            public int test;
        }

        private class User : IValidatableObject
        {
            public string Password { get; set; }

            [Compare("Password")]
            public string ConfirmPassword { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (Password == "password")
                {
                    yield return new ValidationResult("Password does not meet complexity requirements.");
                }
            }
        }

        public interface IExampleService
        {
            void DoSomething();
        }
    }
}
