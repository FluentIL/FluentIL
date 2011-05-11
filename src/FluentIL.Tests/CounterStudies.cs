using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    using NUnit.Framework;
    using SharpTestsEx;

    [TestFixture]
    public class CounterStudies
    {
        
    }
}
