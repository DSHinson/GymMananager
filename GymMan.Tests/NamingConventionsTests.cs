namespace GymMan.Tests.ConventionTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using GymMan.Web;
    using NUnit.Framework;

    [TestFixture]
    public class NamingConventionsTests
    {
        private static readonly Assembly TargetAssembly = typeof(GymMan.Web.App).Assembly;

        private static bool IsPascalCase(string name) => Regex.IsMatch(name, @"^[A-Z][a-zA-Z0-9]*$");

        private static bool IsCamelCase(string name) => Regex.IsMatch(name, @"^[a-z][a-zA-Z0-9]*$");

        private static bool IsPrivateFieldNaming(string name) => Regex.IsMatch(name, @"^_[a-z][a-zA-Z0-9]*$");

        private static bool IsGeneratedField(FieldInfo field) =>
            field.Name.StartsWith("__") ||
            field.Name.Contains("k__BackingField") ||
            field.Name.StartsWith("<") ||
            field.Name.Contains("tagHelper") ||
            field.Name.Contains("u__") ||
            field.Name.Contains("s__");

        private static bool IsGeneratedType(Type type) =>
            type.Name.StartsWith("Pages_") ||
            type.Name.Contains("__") ||
            type.Name.StartsWith("<>") ||
            type.FullName?.Contains("Microsoft.AspNetCore") == true;

        private static bool IsGeneratedMethod(MethodInfo method) =>
            method.Name.Contains("BuildRenderTree") ||
            method.Name.Contains("b__") ||
            method.DeclaringType?.Name.StartsWith("<>") == true ||
            method.Name.StartsWith("<");

        private static bool IsGeneratedParameter(ParameterInfo parameter) => parameter.Name?.StartsWith("__") == true || parameter.Name?.StartsWith("<") == true;

        [Test]
        public void ClassNames_ShouldBePascalCase()
        {

            var invalidClassNames = TargetAssembly.GetTypes()
                .Where(type =>
                    type.IsClass &&
                    !IsPascalCase(type.Name) &&
                    !type.Name.StartsWith("<") &&             // compiler-generated anonymous/async/etc.
                    !type.Name.StartsWith("_") &&             // Razor partial classes or hidden files
                    !type.Name.StartsWith("Pages__") &&       // Razor-generated Pages__Layout etc.
                    !type.FullName.Contains("Pages__Layout") &&
                    !type.FullName.Contains("Pages__Host") &&
                    !type.FullName.Contains("_Imports") &&
                    !type.FullName.Contains("Shared__") &&
                    !type.Name.Contains("c__"))               // anonymous methods / closures
                .Select(type => type.Name)
                .Distinct()
                .ToList();

            CollectionAssert.IsEmpty(invalidClassNames,$"Class names not in PascalCase: {string.Join(", ", invalidClassNames)}");
        }

        [Test]
        public void AllPublicMethodNames_ShouldBePascalCase()
        {
            var violations = TargetAssembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                .Where(m => !m.IsSpecialName && !IsPascalCase(m.Name))
                .Select(m => $"{m.DeclaringType?.Name}.{m.Name}")
                .ToList();

            Assert.That(violations, Is.Empty, $"Public methods not in PascalCase: {string.Join(", ", violations)}");
        }

        [Test]
        public void AllPublicPropertyNames_ShouldBePascalCase()
        {
            var violations = TargetAssembly.GetTypes()
                .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                .Where(p => !IsPascalCase(p.Name))
                .Select(p => $"{p.DeclaringType?.Name}.{p.Name}")
                .ToList();

            Assert.That(violations, Is.Empty, $"Properties not in PascalCase: {string.Join(", ", violations)}");
        }

        [Test]
        public void AllPrivateFieldNames_ShouldStartWithUnderscoreAndBeCamelCase()
        {
            var violations = TargetAssembly.GetTypes()
                .Where(t => !IsGeneratedType(t))
                .SelectMany(t => t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                .Where(f => !f.IsStatic && !IsGeneratedField(f) && !IsPrivateFieldNaming(f.Name))
                .Select(f => $"{f.DeclaringType?.Name}.{f.Name}")
                .ToList();

            Assert.That(violations, Is.Empty, $"Private fields not in _camelCase: {string.Join(", ", violations)}");
        }

        [Test]
        public void AllMethodParameterNames_ShouldBeCamelCase()
        {
            var violations = TargetAssembly.GetTypes()
                .Where(t => !IsGeneratedType(t))
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                .Where(m => !m.IsSpecialName && !IsGeneratedMethod(m))
                .SelectMany(m => m.GetParameters()
                    .Where(p => !IsGeneratedParameter(p) && !IsCamelCase(p.Name))
                    .Select(p => $"{m.DeclaringType?.Name}.{m.Name}({p.Name})"))
                .ToList();

            Assert.That(violations, Is.Empty, $"Method parameters not in camelCase: {string.Join(", ", violations)}");
        }
    }
}
