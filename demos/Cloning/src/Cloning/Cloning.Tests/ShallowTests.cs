using NUnit.Framework;
using SharpTestsEx;

namespace Cloning.Tests
{
    public class Dummy1
    {
        public string v1;
        public string v2;
    }

    public class Dummy2
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }

    public class Derived : Dummy2
    {
        public int Property3 { get; set; }

    }

    [TestFixture]
    public class ShallowTests
    {
        [Test]
        public void CloningBasicDummyObject()
        {
            var d = new Dummy1 {v1 = "Value of V1", v2 = "Value of v2"};
            var dCloned = d.ShallowClone();

            d.v1.Should().Be(dCloned.v1);
            d.v2.Should().Be(dCloned.v2);
        }

        [Test]
        public void CloningDummyObjectWithAutoProperties()
        {
            var d = new Dummy2 { Property1 = "Value of V1", Property2 = "Value of v2" };
            var dCloned = d.ShallowClone();

            d.Property1.Should().Be(dCloned.Property1);
            d.Property2.Should().Be(dCloned.Property2);
        }

        [Test]
        public void CloningAnonymousTypeObject()
        {
            var d = new {Property1 = "Property1", Property2 = 10};
            var dCloned = d.ShallowClone();

            d.Property1.Should().Be(dCloned.Property1);
            d.Property2.Should().Be(dCloned.Property2);
        }

        [Test]
        public void CloningDerivedTypeObject()
        {
            var d = new Derived
                        {
                            Property1 = "Value of V1", 
                            Property2 = "Value of v2", 
                            Property3 = 10
                        };
            var dCloned = d.ShallowClone();

            d.Property1.Should().Be(dCloned.Property1);
            d.Property2.Should().Be(dCloned.Property2);
            d.Property3.Should().Be(dCloned.Property3);
        }
    }
}