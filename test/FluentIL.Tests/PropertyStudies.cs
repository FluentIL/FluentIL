using System;

namespace FluentIL.Tests
{
    using NUnit.Framework;
    using SharpTestsEx;

    [TestFixture]
    public class PropertyStudies
    {
        [Test]
        public void EmittingTypeThatSupportsAutomaticProperties()
        {
            var newType = IL.NewType()
                .Implements<IFoo>()
                .WithAutoProperty("SomeProperty", typeof(int)
                ).AsType;

            var f = (IFoo)Activator.CreateInstance(newType);
            f.SomeProperty = 10;
            f.SomeProperty.Should().Be(10);
        }

        [Test]
        public void EmittingTypeThatSupportsAutomaticPropertiesWithoutInterface()
        {
            var newType = IL.NewType()
                .WithAutoProperty("SomeProperty", typeof(int)
                ).AsType;

            var f = Activator.CreateInstance(newType);
            var p = newType.GetProperty("SomeProperty");
            p.SetValue(f, 10, null);
            p.GetValue(f, null).Should().Be(10);
        }

        [Test]
        public void EmitingTypeThatSupportsReadOnlyProperty()
        {
            var newType = IL.NewType()
                .Implements<IFoo2>()
                .WithProperty("ReadOnlyProperty", typeof(int), (m) => m
                    .Ldc(10)
                    .Ret()
                    )
                .AsType;

            var f = (IFoo2)Activator.CreateInstance(newType);
            f.ReadOnlyProperty.Should().Be(10);
        }

        [Test]
        public void EmitingTypeThatSupportsReadOnlyExplicitlyImplementedProperty()
        {
            var newType = IL.NewType()
                .Implements<IFoo2>()
                .WithProperty("ReadOnlyProperty", typeof(int))
                .ImplementsExplicitly<IFoo2>()
                .WithGetter()
                    .Ldc(10)
                    .Ret()
                .AsType;

            var f = Activator.CreateInstance(newType);
            var p = newType.GetProperty("ReadOnlyProperty");
            ((IFoo2)f).ReadOnlyProperty.Should().Be(10);
            p.Should().Be.Null();
        }

        [Test]
        public void EmitingTypeThatSupportsReadOnlyPropertyWithoutInterface()
        {
            var newType = IL.NewType()
                .WithProperty("ReadOnlyProperty", typeof(int), (m) => m
                    .Ldc(10)
                    .Ret()
                    )
                .AsType;

            var f = Activator.CreateInstance(newType);
            var p = newType.GetProperty("ReadOnlyProperty");
            p.GetValue(f, null).Should().Be(10);
        }



        public interface IFoo2
        {
            int ReadOnlyProperty { get;  }
        }

        public interface IFoo
        {
            int SomeProperty { get; set; }
        }
    }
}
