using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentIL.Tests
{
    using NUnit.Framework;
    using SharpTestsEx;

    [TestFixture]
    public class GetSetStudies
    {
        [Test]
        public void Get_Case1()
        {
            // arrange
            var method = IL.NewMethod()
                .WithParameter(typeof(Foo), "foo")
                .Returns(typeof(string))
                .Ldarg("foo")
                .CallGet<Foo>("Prop")
                .Ret();

            var foo = new Foo();

            // act
            method.Invoke(foo);

            // assert
            foo.PropGetCount.Should().Be(1);
        }

        [Test]
        public void Set_Case1()
        {
            // arrange
            var method = IL.NewMethod()
                .WithParameter(typeof(Foo), "foo")
                .Returns(typeof(void))
                .Ldarg("foo")
                .Ldstr("Hello, World!")
                .CallSet<Foo>("Prop")
                .Ret();

            var foo = new Foo();

            // act
            method.Invoke(foo);

            // assert
            foo.PropSetCount.Should().Be(1);
            foo.Prop.Should().Be("Hello, World!");
        }

        [Test]
        public void Set_Case2()
        {
            // arrange
            var method = IL.NewMethod()
                .WithParameter(typeof(FooLoo), "foo")
                .Returns(typeof(void))
                .Ldarg("foo")
                .CallSet<FooLoo>(new { Name = "Elemar" })
                .Ret();

            var foo = new FooLoo();

            // act
            method.Invoke(foo);

            // assert
            foo.Name.Should().Be("Elemar");
        }

        [Test]
        public void Set_Case3()
        {
            // arrange
            var method = IL.NewMethod()
                .WithParameter(typeof(FooLoo), "foo")
                .Returns(typeof(void))
                .Ldarg("foo")
                .CallSet<FooLoo>( new { 
                    Name = "Elemar", 
                    Age = 31 
                })
                .Ret();

            var foo = new FooLoo();

            // act
            method.Invoke(foo);

            // assert
            foo.Name.Should().Be("Elemar");
            foo.Age.Should().Be(31);
        }

    }

    public class FooLoo
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Live { get; set; }
    }

    public class Foo
    {
        public int PropGetCount { get; private set; }
        public int PropSetCount { get; private set; }

        string prop;
        public string Prop
        {
            get
            {
                PropGetCount++;
                return prop;
            }
            set
            {
                PropSetCount++;
                prop = value;
            }
        }
    }
}
