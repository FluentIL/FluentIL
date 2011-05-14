using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProxy.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var concrete = new Foo();

            var proxy = ProxyBuilder.CreateProxy<IFoo>(
                    concrete,
                    beforeExecuteAction: (s, o) =>
                        Console.WriteLine("Before execute {0}", s),
                    afterExecuteAction: (s, o) =>
                        Console.WriteLine("After execute: {0}", s)
                );

            proxy.SayHello();
            Console.ReadLine();
        }
    }


    public class Foo : IFoo
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public void SayHello()
        {
            Console.WriteLine("Hello World!");
        }
    }

    public interface IFoo
    {
        int Add(int a, int b);
        void SayHello();
    }

    public class FooProxyMonitor : IProxyMonitor
    {

        public void BeforeExecute(string methodName, object[] p)
        {
            Console.WriteLine("Before");
        }

        public void AfterExecute(string methodName, object result)
        {
            Console.WriteLine("After");
        }
    }
}
