using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentIL.Tests
{
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

    public class ConcreteFoo2
    {
        public int FooProperty { get; set; }

        public ConcreteFoo2()
        {
            FooProperty = Int32.MinValue;
        }

        public ConcreteFoo2(int value)
        {
            FooProperty = value;
        }
    }

    public interface IFoo
    {
        int Add(int a, int b);
        int Mul(int a, int b);
    }

    public interface IFoo2
    {
        int IntProperty { get; set; }
    }
}
