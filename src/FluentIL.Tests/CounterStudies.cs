using System;
using System.Reflection.Emit;
using NUnit.Framework;
using SharpTestsEx;

//public class Counter
//{
//    int currentValue = 0;

//    public void Increment()
//    {
//        this.currentValue++;
//    }

//    public void Decrement()
//    {
//        this.currentValue--;
//    }

//    public int GetCurrentValue()
//    {
//        return this.currentValue;
//    }
//}

//.method public hidebysig instance void  Decrement() cil managed
//{
//  // Code size       16 (0x10)
//  .maxstack  8
//  IL_0000:  nop
//  IL_0001:  ldarg.0
//  IL_0002:  dup
//  IL_0003:  ldfld      int32 ConsoleApplication3.Counter::currentValue
//  IL_0008:  ldc.i4.1
//  IL_0009:  sub
//  IL_000a:  stfld      int32 ConsoleApplication3.Counter::currentValue
//  IL_000f:  ret
//} // end of method Counter::Decrement

//.method public hidebysig instance void  Increment() cil managed
//{
//  // Code size       16 (0x10)
//  .maxstack  8
//  IL_0000:  nop
//  IL_0001:  ldarg.0
//  IL_0002:  dup
//  IL_0003:  ldfld      int32 ConsoleApplication3.Counter::currentValue
//  IL_0008:  ldc.i4.1
//  IL_0009:  add
//  IL_000a:  stfld      int32 ConsoleApplication3.Counter::currentValue
//  IL_000f:  ret
//} // end of method Counter::Increment

//.method public hidebysig instance int32  GetCurrentValue() cil managed
//{
//  // Code size       12 (0xc)
//  .maxstack  1
//  .locals init ([0] int32 CS$1$0000)
//  IL_0000:  nop
//  IL_0001:  ldarg.0
//  IL_0002:  ldfld      int32 ConsoleApplication3.Counter::currentValue
//  IL_0007:  stloc.0
//  IL_0008:  br.s       IL_000a
//  IL_000a:  ldloc.0
//  IL_000b:  ret
//} // end of method Counter::GetCurrentValue


namespace FluentIL.Tests
{
    [TestFixture]
    public class CounterStudies
    {
        [Test]
        public void Counter_BasicILVersion()
        {
            var cti = IL.NewType()
                .Implements<ICounter>()
                .WithField("currentValue", typeof(int));

            var field = cti.GetFieldInfo("currentValue");

            cti
                .WithMethod("Increment")
                .Returns(typeof(void))
                    .Emit(OpCodes.Nop)
                    .Ldarg(0)
                    .Dup()
                    .Emit(OpCodes.Ldfld, field)
                    .Ldc(1)
                    .Add()
                    .Emit(OpCodes.Stfld, field)
                    .Ret()
                .WithMethod("Decrement")
                .Returns(typeof(void))
                    .Emit(OpCodes.Nop)
                    .Ldarg(0)
                    .Dup()
                    .Emit(OpCodes.Ldfld, field)
                    .Ldc(1)
                    .Sub()
                    .Emit(OpCodes.Stfld, field)
                    .Ret()
                .WithMethod("GetCurrentValue")
                .WithVariable(typeof(int))
                .Returns(typeof(int))
                    .Emit(OpCodes.Nop)
                    .Ldarg(0)
                    .Emit(OpCodes.Ldfld, field)
                    .Stloc(0)
                    .Br_S("IL_000a")
                    .MarkLabel("IL_000a")
                    .Ldloc(0)
                    .Ret();

            var counter = (ICounter)Activator.CreateInstance(cti.AsType);

            counter.GetCurrentValue().Should().Be(0);
            counter.Increment();
            counter.GetCurrentValue().Should().Be(1);
            counter.Decrement();
            counter.GetCurrentValue().Should().Be(0);
        }

        [Test]
        public void Counter_ReplacingEmits()
        {
            var cti = IL.NewType()
                .Implements<ICounter>()
                .WithField("currentValue", typeof(int));

            var field = cti.GetFieldInfo("currentValue");

            cti
                .WithMethod("Increment")
                .Returns(typeof(void))
                    .Nop()
                    .Ldarg(0)
                    .Dup()
                    .Ldfld(field)
                    .Ldc(1)
                    .Add()
                    .Stfld(field)
                    .Ret()
                .WithMethod("Decrement")
                .Returns(typeof(void))
                    .Nop()
                    .Ldarg(0)
                    .Dup()
                    .Ldfld(field)
                    .Ldc(1)
                    .Sub()
                    .Stfld(field)
                    .Ret()
                .WithMethod("GetCurrentValue")
                .WithVariable(typeof(int))
                .Returns(typeof(int))
                    .Nop()
                    .Ldarg(0)
                    .Ldfld(field)
                    .Stloc(0)
                    .Br_S("IL_000a")
                    .MarkLabel("IL_000a")
                    .Ldloc(0)
                    .Ret();

            var counter = (ICounter)Activator.CreateInstance(cti.AsType);

            counter.GetCurrentValue().Should().Be(0);
            counter.Increment();
            counter.GetCurrentValue().Should().Be(1);
            counter.Decrement();
            counter.GetCurrentValue().Should().Be(0);
        }


        [Test]
        public void Counter_Simplifying()
        {
            var cti = IL.NewType()
                .Implements<ICounter>()
                .WithField("currentValue", typeof(int))
                .WithMethod("Increment")
                .Returns(typeof(void))
                    .Ldarg(0)
                    .Dup()
                    .Ldfld("currentValue")
                    .Add(1)
                    .Stfld("currentValue")
                    .Ret()
                .WithMethod("Decrement")
                .Returns(typeof(void))
                    .Ldarg(0)
                    .Dup()
                    .Ldfld("currentValue")
                    .Sub(1)
                    .Stfld("currentValue")
                    .Ret()
                .WithMethod("GetCurrentValue")
                .Returns(typeof(int))
                    .Ldarg(0)
                    .Ldfld("currentValue")
                    .Ret();

            var counter = (ICounter)Activator.CreateInstance(cti.AsType);

            counter.GetCurrentValue().Should().Be(0);
            counter.Increment();
            counter.GetCurrentValue().Should().Be(1);
            counter.Decrement();
            counter.GetCurrentValue().Should().Be(0);
        }

    }

    public interface ICounter
    {
        void Increment();
        void Decrement();
        int GetCurrentValue();
    }
}
