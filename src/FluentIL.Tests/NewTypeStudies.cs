using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FluentIL.Tests
{
    using NUnit.Framework;
    using SharpTestsEx;

    [TestFixture]
    public class NewTypeStudies
    {
        [Test]
        public void CreateTypeWithOneMethod()
        {
            // arrange
            var t = IL.NewType("One")
                .WithMethod("Method").Returns(typeof(void))
                    .Ret()
                .AsType;

            var instance = Activator.CreateInstance(t);

            instance.GetType().Name.Should().Be("One");
            instance.GetType().GetMethod("Method").Should().Not.Be(null);
        }

        [Test]
        public void CreateTypeThatInheritsFromBaseType()
        {
            var t = IL.NewType("ConcreteFoo2")
                      .Inherits<ConcreteFoo>()
                      .AsType;

            var instance = (IFoo)Activator.CreateInstance(t);

            instance.GetType().BaseType.Should().Be(typeof(ConcreteFoo));
            instance.Add(2, 3).Should().Be(5);
            instance.Mul(2, 3).Should().Be(6);
        }

        [Test]
        public void TwoPlusTwoWithNamedParameters()
        {
            var t = IL.NewType()
                .WithMethod("Add")
                    .WithParameter(typeof(int), "a")
                    .WithParameter(typeof(int), "b")
                    .Returns(typeof(int))
                    .Ldarg("a")
                    .Ldarg("b")
                    .Add()
                    .Ret()
                .AsType;

            var instance = Activator.CreateInstance(t);

            var mi = instance.GetType().GetMethod("Add");
            var result = mi.Invoke(instance, new object[] { 2, 2 });
            result.Should().Be(4);
        }

        [Test]
        public void ClassWithMultAndDivMethods()
        {

            var t = IL.NewType()
                .WithMethod("Add")
                    .WithParameter(typeof(int), "a")
                    .WithParameter(typeof(int), "b")
                    .Returns(typeof(int))
                    .Ldarg("a")
                    .Ldarg("b")
                    .Add()
                    .Ret()
                .WithMethod("Mul")
                    .WithParameter(typeof(int), "a")
                    .WithParameter(typeof(int), "b")
                    .Returns(typeof(int))
                    .Ldarg("a")
                    .Ldarg("b")
                    .Mul()
                    .Ret()
                .AsType;

            var instance = Activator.CreateInstance(t);

            var miAdd = instance.GetType().GetMethod("Add");
            var result = miAdd.Invoke(instance, new object[] { 2, 3 });

            var miMult = instance.GetType().GetMethod("Mul");
            result = miMult.Invoke(instance, new object[] { 2, 3 });
            result.Should().Be(6);
        }

        [Test]
        public void ClassWithMultAndDivMethodsImplementingInterface()
        {

            var t = IL.NewType()
                .Implements<IFoo>()
                .WithMethod("Add")
                    .WithParameter(typeof(int), "a")
                    .WithParameter(typeof(int), "b")
                    .Returns(typeof(int))
                    .Ldarg("a")
                    .Ldarg("b")
                    .Add()
                    .Ret()
                .WithMethod("Mul")
                    .WithParameter(typeof(int), "a")
                    .WithParameter(typeof(int), "b")
                    .Returns(typeof(int))
                    .Ldarg("a")
                    .Ldarg("b")
                    .Mul()
                    .Ret()
                .AsType;

            var instance = (IFoo)Activator.CreateInstance(t);

            instance.Add(2, 3).Should().Be(5);
            instance.Mul(2, 3).Should().Be(6);
        }
    }

    public class ConcreteFoo : IFoo
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Mul(int a, int b)
        {
            return a * b;
        }
    }

    public interface IFoo
    {
        int Add(int a, int b);
        int Mul(int a, int b);
    }
}
