using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using SharpTestsEx;
using System.Reflection.Emit;

namespace FluentIL.Tests
{
    [TestFixture]
    public class VariableStudies
    {
        [Test]
        public void SumOf1To100_Reference()
        {
            var method = IL.NewMethod()
                .Returns(typeof(int))
                .AsDynamicMethod;

            var ilgen = method.GetILGenerator();
            ilgen.DeclareLocal(typeof(int)); // int var1;
            ilgen.DeclareLocal(typeof(int)); // int var2

            ilgen.Emit(OpCodes.Ldc_I4_1);
            ilgen.Emit(OpCodes.Stloc_0);    // var1 = 1;

            var lbl = ilgen.DefineLabel(); // for ...
            ilgen.MarkLabel(lbl);
            
            ilgen.Emit(OpCodes.Ldloc_1);
            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Add);        
            ilgen.Emit(OpCodes.Stloc_1);    // var2 = var2 + var1

            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Ldc_I4_1);
            ilgen.Emit(OpCodes.Add);
            ilgen.Emit(OpCodes.Stloc_0);    // var1 = var1 + 1;

            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Ldc_I4, 100);
            ilgen.Emit(OpCodes.Ble, lbl);  // if (var1 <= 100) ..

            ilgen.Emit(OpCodes.Ldloc_1);
            ilgen.Emit(OpCodes.Ret);    // return var2

            var result = method.Invoke(null, null);

            int expected = 0;
            for (int i = 0; i <= 100; i++)
                expected += i;

            result.Should().Be(expected); // expected = 5550;
        }

        [Test]
        public void SumOf1To100()
        {
            var method = IL.NewMethod()
                .WithVariable(typeof(int), "count")
                .WithVariable(typeof(int), "total")
                .Returns(typeof(int))
            
                .LdcI4(1)
                .Stloc("count")

                .MarkLabel("start")

                .Ldloc("total", "count")
                .Add()
                .Stloc("total")

                .Ldloc("count")
                .Add(1)
                .Stloc("count")

                .Ldloc("count")
                .LdcI4(100)
                .Ble("start")

                .Ldloc("total")
                .Ret();
            
            var result = method.Invoke();

            result.Should().Be(5050); 
        }
        
        [Test]
        public void SumOf1To100_For()
        {
            var method = IL.NewMethod()
                .WithVariable(typeof(int), "total")
                .Returns(typeof(int))

                .For("count", 1, 100, 1)

                    .Ldloc("total", "count")
                    .Add()
                    .Stloc("total")

                .Next()

                .Ldloc("total")
                .Ret();

            var result = method.Invoke();

            result.Should().Be(5050);
        }

        [Test]
        public void SumOf100To1_For()
        {
            var method = IL.NewMethod()
                .WithVariable(typeof(int), "total")
                .Returns(typeof(int))

                .For("count", 100, 1, -1)

                    .Ldloc("total", "count")
                    .Add()
                    .Stloc("total")

                .Next()

                .Ldloc("total")
                .Ret();

            var result = method.Invoke();

            result.Should().Be(5050);
        }


    }
}
