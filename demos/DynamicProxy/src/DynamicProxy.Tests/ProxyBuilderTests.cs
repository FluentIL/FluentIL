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

        [Test]
        public void CreateProxy_BasicProxyMonitorSupport()
        {
            // arrange
            var foo = new Foo();
            var target = ProxyBuilder.CreateProxy<IFoo>(
                foo,
                new DummyProxyMonitor()
                );
            // act
            var result = target.Add(2, 3);
            // assert
            foo.AddHits.Should().Be(1);
            result.Should().Be(5);
        }

        [Test]
        public void CreateProxy_AfterExecute_Add()
        {
            // arrange
            var foo = new Foo();
            var monitor = new DummyProxyMonitor();
            //Assert.Fail(string.Format("LastResult {0}", monitor.AfterExecute_LastResult));
            var target = ProxyBuilder.CreateProxy<IFoo>(
                foo,
                monitor
                );
            // act
            var result = target.Add(2, 3);
            // assert
            monitor.AfterExecute_LastMethodName.Should().Be("Add");
            monitor.AfterExecute_LastResult.Should().Be(5);
        }

        [Test]
        public void CreateProxy_AfterExecute_MethodWithNoParameters()
        {
            // arrange
            var foo = new Foo();
            var monitor = new DummyProxyMonitor();
            //Assert.Fail(string.Format("LastResult {0}", monitor.AfterExecute_LastResult));
            var target = ProxyBuilder.CreateProxy<IFoo>(
                foo,
                monitor
                );
            // act
            target.MethodWithNoParameters();
            // assert
            monitor.AfterExecute_LastMethodName.Should().Be("MethodWithNoParameters");
            monitor.AfterExecute_LastResult.Should().Be(null);
        }

        [Test]
        public void CreateProxy_BeforeExecute_MethodWithNoParameters()
        {
            // arrange
            var foo = new Foo();
            var monitor = new DummyProxyMonitor();
            //Assert.Fail(string.Format("LastResult {0}", monitor.AfterExecute_LastResult));
            var target = ProxyBuilder.CreateProxy<IFoo>(
                foo,
                monitor
                );
            // act
            target.MethodWithNoParameters();
            // assert
            monitor.BeforeExecute_LastMethodName.Should().Be("MethodWithNoParameters");
            //monitor.AfterExecute_LastResult.Should().Be(null);
        }

        [Test]
        public void CreateProxy_BeforeExecute_Add()
        {
            // arrange
            var foo = new Foo();
            var monitor = new DummyProxyMonitor();
            //Assert.Fail(string.Format("LastResult {0}", monitor.AfterExecute_LastResult));
            var target = ProxyBuilder.CreateProxy<IFoo>(
                foo,
                monitor
                );
            // act
            target.Add(2, 3);
            // assert
            monitor.BeforeExecute_LastMethodName.Should().Be("Add");
            monitor.BeforeExecute_LastParameters.Should().Have.SameSequenceAs(
                2, 3);
        }
    }

    class DummyProxyMonitor : IProxyMonitor
    {
        public string BeforeExecute_LastMethodName;
        public object[] BeforeExecute_LastParameters;
        
        public void BeforeExecute(string methodName, object[] p)
        {
            BeforeExecute_LastMethodName = methodName;
            BeforeExecute_LastParameters = p;
        }

        public string AfterExecute_LastMethodName;
        public object AfterExecute_LastResult;

        public void AfterExecute(string methodName, object result)
        {
            AfterExecute_LastMethodName = methodName;
            AfterExecute_LastResult = result;
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
// ----------------------------------------------------
//.class NewType457d494a-f83c-489b-b103-6258fb403c80
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
//    ldarg.1
//    ldarg.2
//    call Int32 Add(Int32, Int32)
//    ret
// ----------------------------------------------------
//.class NewType5d217ee7-a7bf-4a78-a303-6a7d3d2ab1fc
//implements DynamicProxy.Tests.IFoo
//.field (DynamicProxy.Tests.IFoo) __concreteinstance
//.method __SetConcreteInstance
//.param (1) [DynamicProxy.Tests.IFoo] no-name
//returns System.Void
//    ldarg.0
//    ldarg.1
//    stfld __concreteinstance
//    ret
//.field (DynamicProxy.IProxyMonitor) __proxymonitor
//.method __SetProxyMonitor
//.param (1) [DynamicProxy.IProxyMonitor] no-name
//returns System.Void
//    ldarg.0
//    ldarg.1
//    stfld __proxymonitor
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
//    ldarg.1
//    ldarg.2
//    call Int32 Add(Int32, Int32)
//    ret
// ----------------------------------------------------
//.class NewType32f43943-4daa-4393-8835-8dedbbe8f12c
//implements DynamicProxy.Tests.IFoo
//.field (DynamicProxy.Tests.IFoo) __concreteinstance
//.method __SetConcreteInstance
//.param (1) [DynamicProxy.Tests.IFoo] no-name
//returns System.Void
//    ldarg.0
//    ldarg.1
//    stfld __concreteinstance
//    ret
//.field (DynamicProxy.IProxyMonitor) __proxymonitor
//.method __SetProxyMonitor
//.param (1) [DynamicProxy.IProxyMonitor] no-name
//returns System.Void
//    ldarg.0
//    ldarg.1
//    stfld __proxymonitor
//    ret
//.method MethodWithNoParameters
//.local (0) [System.Void] no-name
//returns System.Void
//    ldarg.0
//    dup
//    ldfld __proxymonitor
//    ldstr "MethodWithNoParameters"
//    ldarg.0
//    ldfld __concreteinstance
//    call Void MethodWithNoParameters()
//    stloc.0
//    ldloc.0
//    box System.Void
//    call Void AfterExecute(System.String, System.Object)
//    ldloc.0
//    ret
//.method Add
//.param (1) [System.Int32] a
//.param (2) [System.Int32] b
//.local (0) [System.Int32] no-name
//returns System.Int32
//    ldarg.0
//    dup
//    ldfld __proxymonitor
//    ldstr "Add"
//    ldarg.0
//    ldfld __concreteinstance
//    ldarg.1
//    ldarg.2
//    call Int32 Add(Int32, Int32)
//    stloc.0
//    ldloc.0
//    box System.Int32
//    call Void AfterExecute(System.String, System.Object)
//    ldloc.0
//    ret
//-----------------------------------------------------
//.class NewTypee362ab79-005d-4c09-8944-b8c1cdd2e94c
//implements DynamicProxy.Tests.IFoo
//.field (DynamicProxy.Tests.IFoo) __concreteinstance
//.method __SetConcreteInstance
//.param (1) [DynamicProxy.Tests.IFoo] no-name
//returns System.Void
//    ldarg.0
//    ldarg.1
//    stfld __concreteinstance
//    ret
//.field (DynamicProxy.IProxyMonitor) __proxymonitor
//.method __SetProxyMonitor
//.param (1) [DynamicProxy.IProxyMonitor] no-name
//returns System.Void
//    ldarg.0
//    ldarg.1
//    stfld __proxymonitor
//    ret
//.method MethodWithNoParameters
//returns System.Void
//    ldarg.0
//    dup
//    ldfld __proxymonitor
//    ldstr "MethodWithNoParameters"
//    ldarg.0
//    ldfld __concreteinstance
//    call Void MethodWithNoParameters()
//    ldnull
//    call Void AfterExecute(System.String, System.Object)
//    ret
//.method Add
//.param (1) [System.Int32] a
//.param (2) [System.Int32] b
//.local (0) [System.Int32] no-name
//returns System.Int32
//    ldarg.0
//    dup
//    ldfld __proxymonitor
//    ldstr "Add"
//    ldarg.0
//    ldfld __concreteinstance
//    ldarg.1
//    ldarg.2
//    call Int32 Add(Int32, Int32)
//    stloc.0
//    ldloc.0
//    box System.Int32
//    call Void AfterExecute(System.String, System.Object)
//    ldloc.0
//    ret