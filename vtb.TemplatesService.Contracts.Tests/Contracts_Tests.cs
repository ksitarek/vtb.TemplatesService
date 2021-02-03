using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Shouldly;
using vtb.TemplatesService.Contracts.Responses;

namespace vtb.TemplatesService.Contracts.Tests
{
    public class Contracts_Tests
    {
        private static readonly Assembly _assembly = typeof(DefaultTemplateResponse).Assembly;
        private static readonly Type[] AllContractTypes = _assembly.GetExportedTypes();
        private static readonly Type objectType = typeof(object);

        [TestCaseSource(nameof(AllContractTypes))]
        public void All_Contract_Types_Are_Plain_Records(Type @type)
        {
            type.IsClass.ShouldBeTrue(@type.FullName);
            type.IsValueType.ShouldBeFalse(@type.FullName);
            type.BaseType.ShouldBe(objectType, @type.FullName);
        }

        [TestCaseSource(nameof(AllContractTypes))]
        public void All_Contract_Types_Are_POCOs(Type @type)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var methods = @type.GetMethods(bindingFlags)
                .Where(x => !(x.IsSpecialName && x.Name.StartsWith("get_")))
                .Select(m => m.ToString());

            var allowedMethods = objectType.GetMethods(bindingFlags).Select(m => m.ToString());
            var remainingMethods = methods.Where(m => !allowedMethods.Contains(m));

            remainingMethods.Count().ShouldBe(0, @type.FullName);
        }

        [TestCaseSource(nameof(AllContractTypes))]
        public void All_Contract_Types_Have_Read_Only_Properties(Type @type)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var properties = @type.GetProperties(bindingFlags);

            var propertiesWithPublicGetters = properties.Where(x => x.GetMethod.IsPublic);
            var propertiesWithSetters = properties.Where(x => x.SetMethod != null);

            propertiesWithPublicGetters.Count().ShouldBe(properties.Length, @type.FullName);
            propertiesWithSetters.Count().ShouldBe(0, @type.FullName);
        }

        [TestCaseSource(nameof(AllContractTypes))]
        public void No_Contract_Types_Have_Parameterless_Constructor(Type @type)
        {
            var parameterlessConstructor = @type.GetConstructor(Type.EmptyTypes);
            parameterlessConstructor.ShouldBeNull(@type.FullName);
        }

        [TestCaseSource(nameof(AllContractTypes))]
        public void All_Contract_Types_Have_Only_Single_Constructor(Type @type)
        {
            @type.GetConstructors().Length.ShouldBe(1);
        }
    }
}