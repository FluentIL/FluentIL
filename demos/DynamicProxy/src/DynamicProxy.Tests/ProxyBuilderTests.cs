using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProxy.Tests
{
    using NUnit.Framework;
    using SharpTestsEx;

    [TestFixture]
    public class ProxyBuilderTests
    {
        [Test]
        public void CreateProxy_PassingInterface_ReturnsInstanceOf()
        {
            // arrange
            var foo = new Foo();
            // act
            var target = ProxyBuilder.CreateProxy<IFoo>(foo);
            // assert
            target.Should().Not.Be.Null();
        }

        [Test]
        public void CreateProxy_CallingMethodsWithNoParameters_CallsConcreteImplementation()
        {
            // arrange
            var foo = new Foo();
            var target = ProxyBuilder.CreateProxy<IFoo>(foo);
            // act
            target.MethodWithNoParameters();
            // assert
            foo.MethodWithNoParametersHits.Should().Be(1);
        }

        [Test]
        public void CreateProxy_CallingMethodsWithParameters_CallsConcreteImplementation()
        {
            // arrange
            var foo = new Foo();
            var target = ProxyBuilder.CreateProxy<IFoo>(foo);
            // act
            var result = target.Add(2, 3);
            // assert
            foo.AddHits.Should().Be(1);
            result.Should().Be(5);
        }
    }

    class Foo : IFoo
    {
        public int MethodWithNoParametersHits { get; set; }
        public int AddHits { get; set; }

        public void MethodWithNoParameters()
        {
            this.MethodWithNoParametersHits++;
        }

        public int Add(int a, int b)
        {
            this.AddHits++;
            return a + b;
        }
    }

    public interface IFoo
    {
        void MethodWithNoParameters();
        int Add(int a, int b);
    }
}

// ----------------------------------------------------
//.class NewType6e405a93-c07d-4fcf-8d94-03de2705b724
//implements DynamicProxy.Tests.IFoo
//.method MethodWithNoParameters
//returns System.Void
//    newobj [System.NotImplementedException] Void .ctor()
//    throw
//    ret
//.method Add
//.param (1) [System.Int32] a
//.param (2) [System.Int32] b
//returns System.Int32
//    newobj [System.NotImplementedException] Void .ctor()
//    throw
//    ret
// ----------------------------------------------------
//.class NewType2a867eb3-9fbd-488b-b207-a65b03f19313
//implements DynamicProxy.Tests.IFoo
//.field (DynamicProxy.Tests.IFoo) __concreteinstance
//.method __SetConcreteInstance
//.param (1) [DynamicProxy.Tests.IFoo] no-name
//returns System.Void
//    ldarg.0
//    ldarg.1
//    stfld __concreteinstance
//    ret
//.method MethodWithNoParameters
//returns System.Void
//    newobj [System.NotImplementedException] Void .ctor()
//    throw
//    ret
//.method Add
//.param (1) [System.Int32] a
//.param (2) [System.Int32] b
//returns System.Int32
//    newobj [System.NotImplementedException] Void .ctor()
//    throw
//    ret
// ----------------------------------------------------
//.class NewTypeb92fed1d-3e5f-4c41-ada7-2677e0da2d22
//implements DynamicProxy.Tests.IFoo
//.field (DynamicProxy.Tests.IFoo) __concreteinstance
//.method __SetConcreteInstance
//.param (1) [DynamicProxy.Tests.IFoo] no-name
//returns System.Void
//    ldarg.0
//    ldarg.1
//    stfld __concreteinstance
//    ret
//.method MethodWithNoParameters
//returns System.Void
//    ldarg.0
//    ldfld __concreteinstance
//    call Void MethodWithNoParameters()
//    ret
//.method Add
//.param (1) [System.Int32] a
//.param (2) [System.Int32] b
//returns System.Int32
//    ldarg.0
//    ldfld __concreteinstance
//    call Int32 Add(Int32, Int32)
//    ret
