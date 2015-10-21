using FluentIL.Infos;
using NUnit.Framework;
using SharpTestsEx;
using System;

namespace FluentIL.Tests
{
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
        public void ClassWithMultAndDivMethods_AlternativeSyntax()
        {
            var t = IL.NewType()
                .WithMethod("Add", AddMethodDefinition)
                .WithMethod("Mul", MultiplyMethodDefinition)                
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

        [Test]
        public void NewType_ParameterlessConstructorDefinition()
        {
            const int PROPERTY_VALUE = 1234;
            var t = IL.NewType()
                .Implements<IFoo2>()
                .WithAutoProperty("IntProperty", typeof(int))
                .WithConstructor(c => c.BodyDefinition()
                                          .Ldarg(0)
                                          .LdcI4(PROPERTY_VALUE)
                                          .CallSet<IFoo2>("IntProperty")
                                       .Ret())
                .AsType;

            var instance = (IFoo2)Activator.CreateInstance(t);
            instance.IntProperty.Should().Be(PROPERTY_VALUE);
        }

        [Test]
        public void NewType_ConstructorWithParameterDefinition()
        {
            const int PROPERTY_VALUE = 1234;
            var t = IL.NewType()
                .Implements<IFoo2>()
                .WithAutoProperty("IntProperty", typeof(int))
                .WithConstructor(c => c.BodyDefinition()
                                          .Ldarg(0)
                                          .Ldarg(1)
                                          .CallSet<IFoo2>("IntProperty")
                                       .Ret(),
                                 argumentTypes:typeof(int))
                .AsType;

            var instance = (IFoo2)Activator.CreateInstance(t, PROPERTY_VALUE);
            instance.IntProperty.Should().Be(PROPERTY_VALUE);
        }

        [Test]
        public void NewType_ConstructorWithParameterAndVariablesDefinition()
        {
            const int X = 11;
            const int Y = 12;
            var t = IL.NewType()
                .Implements<IFoo2>()
                .WithAutoProperty("IntProperty", typeof(int))
                .WithConstructor(c => c.WithVariable<int>("answer")
                                       .BodyDefinition()
                                            .Ldarg(1)
                                            .Ldarg(2)
                                            .Add()
                                            .Stloc("answer")
                                            .Ldarg(0)
                                            .Ldloc("answer")
                                            .CallSet<IFoo2>("IntProperty")
                                       .Ret(),
                                 argumentTypes: new[]{ typeof(int), typeof(int) })
                .AsType;

            var instance = (IFoo2)Activator.CreateInstance(t, X,Y);
            instance.IntProperty.Should().Be(X + Y);
        }

        [Test]
        public void NewType_TwoConstructorsDefinition()
        {
            const int X = 11;
            const int Y = 12;
            var t = IL.NewType()
                .Implements<IFoo2>()
                .WithAutoProperty("IntProperty", typeof(int))
                .WithConstructor(c => c.BodyDefinition()
                                            .Ldarg(0)
                                            .Ldarg(1)
                                            .CallSet<IFoo2>("IntProperty")
                                       .Ret(),
                                 argumentTypes: typeof(int))
                .WithConstructor(c => c.WithVariable<int>("answer")
                                       .BodyDefinition()
                                            .Ldarg(1)
                                            .Ldarg(2)
                                            .Add()
                                            .Stloc("answer")
                                            .Ldarg(0)
                                            .Ldloc("answer")
                                            .CallSet<IFoo2>("IntProperty")
                                       .Ret(),
                                 argumentTypes: new[] { typeof(int), typeof(int) })
                .AsType;

            var instance1 = (IFoo2)Activator.CreateInstance(t, X, Y);
            instance1.IntProperty.Should().Be(X + Y);

            var instance2 = (IFoo2)Activator.CreateInstance(t, X);
            instance2.IntProperty.Should().Be(X);
        }

        [Test]
        public void NewType_CallDefaultBaseConstructor()
        {
            var concreteFoo2 = new ConcreteFoo2();
            var t = IL.NewType("ConcreteFoo3")
                      .Inherits<ConcreteFoo2>()
                      .WithConstructor(c => c.BodyDefinitionWithDefaultBaseCtor()
                                             .Ret())
                      .AsType;

            var instance = (ConcreteFoo2)Activator.CreateInstance(t);
            instance.FooProperty.Should().Be(concreteFoo2.FooProperty);
        }

        [Test]
        public void NewType_CallNonDefaultBaseConstructor()
        {
            const int CONSTRUCTOR_PARAM = 15;
            var t = IL.NewType()
                      .Inherits<ConcreteFoo2>()
                      .WithConstructor(c => c.WithVariable<int>("result")
                                             .BodyDefinition()             
                                             .Ldarg(1)
                                             .Ldarg(1)
                                             .Mul()
                                             .Stloc("result")
                                             .Ldarg(0)
                                             .Ldloc("result")
                                             .Call(c.BaseConstructor(parameterTypes:typeof(int)))
                                             .Ret(),
                                       argumentTypes: typeof(int))
                      .AsType;

            var instance = (ConcreteFoo2)Activator.CreateInstance(t, CONSTRUCTOR_PARAM);
            instance.FooProperty.Should().Be(Math.Pow(CONSTRUCTOR_PARAM,2));
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void NewType_CallNonExistingBaseConstructor()
        {
            IL.NewType()
              .WithConstructor(c => c.BodyDefinition()
                                     .Ldarg(0)
                                     .Ldarg(1)
                                     .Call(c.BaseConstructor(typeof(int)))
                                     .Ret());
        }

        #region Helper Methods

        private static void AddMethodDefinition(DynamicMethodInfo m)
        {
            m.WithParameter(typeof(int), "a")
             .WithParameter(typeof(int), "b")
             .Returns(typeof(int))
             .Ldarg("a")
             .Ldarg("b")
             .Add()
             .Ret();
        }

        private static void MultiplyMethodDefinition(DynamicMethodInfo m)
        {
            m.WithParameter(typeof(int), "a")
             .WithParameter(typeof(int), "b")
             .Returns(typeof(int))
             .Ldarg("a")
             .Ldarg("b")
             .Mul()
             .Ret();
        }

        #endregion
    }
    

}
