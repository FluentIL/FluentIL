using System;
using System.Linq;
using FluentIL.Cecil;
using Mono.Cecil;
using NUnit.Framework;
using SharpTestsEx;

namespace FluentIL.Tests
{
    [TestFixture]
    public class CecilTests
    {
        [Test]
        public void CanAddNewMethodToType()
        {
            var assemblyName = new AssemblyNameDefinition("TempAssembly", new Version(1, 0));
            var assembly = AssemblyDefinition.CreateAssembly(assemblyName, "TempModule", ModuleKind.Dll);

            var typeDefinition = new TypeDefinition("TempNamespace", "TempType", TypeAttributes.Public);

            typeDefinition
                .NewMethod("TempMethod", MethodAttributes.Public, typeof(string), assembly)
                .Nop()
                .Ldstr("Ola")
                .Ret();

            var methodDefinition = typeDefinition.Methods.First(x => x.Name == "TempMethod");
            methodDefinition.Body.Instructions.Count.Should().Be.EqualTo(3);
        }
    }
}
