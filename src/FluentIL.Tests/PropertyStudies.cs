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

        
        public interface IFoo
        {
            int SomeProperty { get; set; }
        }
    }
}
