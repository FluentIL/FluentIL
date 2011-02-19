using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using SharpTestsEx;

namespace FluentIL.Tests
{
    [TestFixture]
    public class MathStudies
    {
        
        [Test]
        public void TwoPlusTwoWithParameters_Reference()
        {
            // arrange
            DynamicMethod dm = new DynamicMethod("SomeName", typeof(int), new Type[] { typeof(int), typeof(int) });

            var ilgen = dm.GetILGenerator();
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Add);
            ilgen.Emit(OpCodes.Ret);

            // act
            var result = dm.Invoke(null, new object[] { 2, 2 });

            // assert
            result.Should().Be(4);
        }

        [Test]
        public void TwoPlusTwoWithParameters()
        {
            // arrange
            var dm = IL.NewMethod()
                .WithParameter(typeof(int))
                .WithParameter(typeof(int))
                .Returns(typeof(int))
                
                .Ldarg(0, 1)
                .Add()
                .Ret();
                
            // act
            var result = dm.Invoke(2, 2);

            // assert
            result.Should().Be(4);
        }

        [Test]
        public void TwoPlusTwoWithNamedParameters()
        {
            // arrange
            var dm = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .WithParameter(typeof(int), "b")
                .Returns(typeof(int))

                .Ldarg("a", "b")
                .Add()
                .Ret();

            // act
            var result = dm.Invoke(2, 2);

            // assert
            result.Should().Be(4);
        }

        [Test]
        public void TwoPlusTwoWithParameters_2()
        {
            // arrange
            var dm = IL.NewMethod(typeof(int), typeof(int), typeof(int))
                .Ldarg(0, 1)
                .Add()
                .Ret();

            // act
            var result = dm.Invoke(2, 2);

            // assert
            result.Should().Be(4);
        }

        [Test]
        public void TwoPlusTwo_Reference()
        {
            // arrange
            var dm = IL.NewMethod(typeof(int))
                .Emit(OpCodes.Ldc_I4_2)
                .Emit(OpCodes.Ldc_I4_2)
                .Add()
                .Ret();

            // act
            var result = dm.Invoke();

            // assert
            result.Should().Be(4);
        }
        
        [Test]
        public void TwoPlusTwo()
        {
            // arrange
            var dm = IL.NewMethod(typeof(int))
                .LdcI4(2, 2)
                .Add()
                .Ret();

            // act
            var result = dm.Invoke();

            // assert
            result.Should().Be(4);
        }

        [Test]
        public void TwoPlusTwo_2()
        {
            // arrange
            var dm = IL.NewMethod(typeof(int))
                .LdcI4(2)
                .Add(2)
                .Ret();

            // act
            var result = dm.Invoke();

            // assert
            result.Should().Be(4);
        }
        
        
        [Test]
        public void TwoPlusTwo_3()
        {
            // arrange
            var dm = IL.NewMethod(typeof(int))
                .Add(2,2)
                .Ret();

            // act
            var result = dm.Invoke();

            // assert
            result.Should().Be(4);
        }

        [Test]
        public void TwoPlusTwo_4()
        {
            // arrange
            var dm = IL.NewMethod(typeof(int))
                .LdcI4(2)
                .Dup()
                .Add()
                .Ret();

            // act
            var result = dm.Invoke();

            // assert
            result.Should().Be(4);
        }



        [Test]
        public void TwoMulThree()
        {
            // arrange
            var dm = IL.NewMethod(typeof(int))
                .LdcI4(2, 3)
                .Mul()
                .Ret();

            // act
            var result = dm.Invoke();

            // assert
            result.Should().Be(6);
        }

        [Test]
        public void TwoMulThree_2()
        {
            // arrange
            var dm = IL.NewMethod(typeof(int))
                .LdcI4(2)
                .Mul(3)
                .Ret();

            // act
            var result = dm.Invoke();

            // assert
            result.Should().Be(6);
        }

        [Test]
        public void TwoMulThree_3()
        {
            // arrange
            var dm = IL.NewMethod(typeof(int))
                .Mul(2, 3)
                .Ret();

            // act
            var result = dm.Invoke();

            // assert
            result.Should().Be(6);
        }

        [Test]
        public void MinusOnePlus40()
        {
            // arrange
            var dm = IL.NewMethod(typeof(int))
                .LdcI4(-1, 40)
                .Add()
                .Ret();

            // act
            var result = dm.Invoke();

            // assert
            result.Should().Be(39);
        }

        [Test]
        public void ThreeTimesFive()
        {
            var result = IL.NewMethod(typeof(int))
                .Mul(3, 5)
                .Ret()
                .Invoke();

            result.Should().Be(15);

        }

        [Test]
        public void AddToVar_Reference()
        {
            var result = IL.NewMethod()
                .WithVariable(typeof(int), "a")
                .Returns(typeof(int))

                .Ldc(10)
                .Stloc("a")

                .Ldloc("a")
                .Add(15)
                .Stloc("a")

                .Ldloc("a")
                .Ret()
                .Invoke();

            result.Should().Be(25);
        }

        [Test]
        public void AddToVar_Reference2()
        {
            var result = IL.NewMethod()
                .WithVariable(typeof(int), "a")
                .Returns(typeof(int))

                .Ldc(10)
                .Stloc("a")

                .Ldc(15)
                .Ldloc("a")
                .Add()
                .Stloc("a")

                .Ldloc("a")
                .Ret()
                .Invoke();

            result.Should().Be(25);
        }

        [Test]
        public void AddToVar_2()
        {
            var result = IL.NewMethod()
                .WithVariable(typeof(int), "a")
                .Returns(typeof(int))

                .Ldc(10)
                .Stloc("a")

                .Ldc(15)
                .AddToVar("a")

                .Ldloc("a")
                .Ret()
                .Invoke();

            result.Should().Be(25);
        }

        [Test]
        public void AddToVar()
        {
            var result = IL.NewMethod()
                .WithVariable(typeof(int), "a")
                .Returns(typeof(int))

                .Ldc(10)
                .Stloc("a")

                .AddToVar("a", 15)

                .Ldloc("a")
                .Ret()
                .Invoke();

            result.Should().Be(25);
        }
    
    }
}
