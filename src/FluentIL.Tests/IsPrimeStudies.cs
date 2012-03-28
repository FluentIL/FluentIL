using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FluentIL.Tests
{
    using NUnit.Framework;
    using SharpTestsEx;
    using System.Reflection.Emit;

    /*
public bool IsPrime(int number)
{
    if (number <= 2) return false;
    for (int i = 2; i <= number / 2; i++)
        if ((number % i) == 0) return false;
    return true;
}
    */

    /*
.method public hidebysig instance bool  IsPrime(int32 number) cil managed
{
  // Code size       29 (0x1d)
  .maxstack  3
  .locals init ([0] int32 i)
  IL_0000:  ldarg.1
  IL_0001:  ldc.i4.1
  IL_0002:  bgt.s      IL_0006
  IL_0004:  ldc.i4.0
  IL_0005:  ret
  IL_0006:  ldc.i4.2
  IL_0007:  stloc.0
  IL_0008:  br.s       IL_0015
  IL_000a:  ldarg.1
  IL_000b:  ldloc.0
  IL_000c:  rem
  IL_000d:  brtrue.s   IL_0011
  IL_000f:  ldc.i4.0
  IL_0010:  ret
  IL_0011:  ldloc.0
  IL_0012:  ldc.i4.1
  IL_0013:  add
  IL_0014:  stloc.0
  IL_0015:  ldloc.0
  IL_0016:  ldarg.1
  IL_0017:  ldc.i4.2
  IL_0018:  div
  IL_0019:  ble.s      IL_000a
  IL_001b:  ldc.i4.1
  IL_001c:  ret
} // end of method Program::IsPrime
    */


    [TestFixture]
    public class IsPrimeStudies
    {
        // adicionei suporte a variáveis
        public IPrimeChecker CreatePrimeCheckerV1()
        {
            var t = IL.NewType().Implements<IPrimeChecker>()
                .WithMethod("IsPrime")
                .WithVariable(typeof(int), "i")
                .WithParameter(typeof(int), "number")
                .Returns(typeof(bool))
                    .Ldarg("number")    // IL_0000:  ldarg.1
                    .Ldc(1)             // IL_0001:  ldc.i4.1
                    .Bgt_S("IL_0006")   // IL_0002:  bgt.s      IL_0006
                    .Ldc(0)             // IL_0004:  ldc.i4.0
                    .Ret()              // IL_0005:  ret
                    .MarkLabel("IL_0006").Ldc(2) // IL_0006:  ldc.i4.2
                    .Stloc("i")          // IL_0007:  stloc.0
                    .Br_S("IL_0015")    // IL_0008:  br.s       IL_0015
                    .MarkLabel("IL_000a").Ldarg("number") // IL_000a:  ldarg.1
                    .Ldloc("i")         // IL_000b:  ldloc.0
                    .Emit(OpCodes.Rem)  // IL_000c:  rem
                    .Brtrue_S("IL_0011")// IL_000d:  brtrue.s   IL_0011
                    .Ldc(0)             // IL_000f:  ldc.i4.0
                    .Ret()              // IL_0010:  ret
                    .MarkLabel("IL_0011").Ldloc("i") // IL_0011:  ldloc.0
                    .Ldc(1)             // IL_0012:  ldc.i4.1
                    .Add()              // IL_0013:  add
                    .Stloc("i")         // IL_0014:  stloc.0
                    .MarkLabel("IL_0015").Ldloc("i") // IL_0015:  ldloc.0
                    .Ldarg("number")    // IL_0016:  ldarg.1
                    .Ldc(2)             // IL_0017:  ldc.i4.2
                    .Div()              // IL_0018:  div
                    .Ble("IL_000a")     // IL_0019:  ble.s      IL_000a
                    .Ldc(1)             // IL_001b:  ldc.i4.1
                    .Ret()              // IL_001c:  ret
                .AsType;

            return (IPrimeChecker)Activator.CreateInstance(t);
        }

        [Test]
        public void IsPrimeV1_Passing0_ReturnsFalse()
        {
            var checker = CreatePrimeCheckerV1();
            checker.IsPrime(0).Should().Be(false);
        }

        [Test]
        public void IsPrimeV1_Passing1_ReturnsFalse()
        {
            var checker = CreatePrimeCheckerV1();
            checker.IsPrime(1).Should().Be(false);
        }

        [Test]
        public void IsPrimeV1_Passing2_Returnstrue()
        {
            var checker = CreatePrimeCheckerV1();
            checker.IsPrime(2).Should().Be(true);
        }

        [Test]
        public void IsPrimeV1_Passing3_ReturnsTrue()
        {
            var checker = CreatePrimeCheckerV1();
            checker.IsPrime(3).Should().Be(true);
        }

        [Test]
        public void IsPrimeV1_Passing4_ReturnsFalse()
        {
            var checker = CreatePrimeCheckerV1();
            checker.IsPrime(4).Should().Be(false);
        }


        // Ret
        // stloc
        // addtovar
        public IPrimeChecker CreatePrimeCheckerV2()
        {
            var t = IL.NewType().Implements<IPrimeChecker>()
                .WithMethod("IsPrime")
                .WithVariable(typeof(int), "i")
                .WithParameter(typeof(int), "number")
                .Returns(typeof(bool))
                    .Ldarg("number")
                    .IfNotgt(1)
                        .Ret(false)
                    .EndIf()

                    .Stloc(2, "i")                      // for
                    .Br_S("IL_0015")                    // for 
                    .MarkLabel("IL_000a")               // for

                    .Ldarg("number")                    // number % i
                    .Ldloc("i")                         // number % i
                    .Rem()                              // number % i

                    .Brtrue_S("IL_0011")                // if
                    .Ret(false)
                    .MarkLabel("IL_0011")               // endif

                    .AddToVar("i", 1)                   // for
                    .MarkLabel("IL_0015").Ldloc("i")    // for
                    .Ldarg("number")                    // for
                    .Div(2)                             // for
                    .Ble("IL_000a")                     // for
                    .Ret(true)
                .AsType;

            return (IPrimeChecker)Activator.CreateInstance(t);
        }

        [Test]
        public void IsPrimeV2_PassingOk()
        {
            var checker = CreatePrimeCheckerV2();
            checker.IsPrime(0).Should().Be(false);
            checker.IsPrime(1).Should().Be(false);
            checker.IsPrime(2).Should().Be(true);
            checker.IsPrime(3).Should().Be(true);
            checker.IsPrime(4).Should().Be(false);
        }


        public IPrimeChecker CreatePrimeCheckerV3()
        {
            var t = IL.NewType().Implements<IPrimeChecker>()
                .WithMethod("IsPrime")
                .WithVariable(typeof(int), "i")
                .WithVariable(typeof(int), "half")
                .WithParameter(typeof(int), "number")
                .Returns(typeof(bool))
                    .Ldarg("number")
                    .IfNotgt(1)
                        .Ret(false)
                    .EndIf()

                    .Ldarg("number")
                    .Div(2)
                    .Stloc("half")

                    .For("i", 2, "half")

                        .Ldarg("number")
                        .Rem("i")

                        .Ifeq(0)
                            .Ret(false)
                        .EndIf()

                    .Next()
                    .Ret(true)
                .AsType;

            return (IPrimeChecker)Activator.CreateInstance(t);
        }

        [Test]
        public void IsPrimeV3_PassingOk()
        {
            var checker = CreatePrimeCheckerV3();
            checker.IsPrime(0).Should().Be(false);
            checker.IsPrime(1).Should().Be(false);
            checker.IsPrime(2).Should().Be(true);
            checker.IsPrime(3).Should().Be(true);
            checker.IsPrime(4).Should().Be(false);
        }


        public IPrimeChecker CreatePrimeCheckerV4()
        {
            var t = IL.NewType().Implements<IPrimeChecker>()
                .WithMethod("IsPrime")
                .WithVariable(typeof(int), "i")
                .WithParameter(typeof(int), "number")
                .Returns(typeof(bool))
                    .If("number<=1")
                        .Ret(false)
                    .EndIf()
                    .For("i", 2, "number/2")
                        .If("(number%i)==0")
                            .Ret(false)
                        .EndIf()
                    .Next()
                    .Ret(true)
                .AsType;

            return (IPrimeChecker)Activator.CreateInstance(t);
        }

        [Test]
        public void IsPrimeV4_PassingOk()
        {
            var checker = CreatePrimeCheckerV4();
            checker.IsPrime(0).Should().Be(false);
            checker.IsPrime(1).Should().Be(false);
            checker.IsPrime(2).Should().Be(true);
            checker.IsPrime(3).Should().Be(true);
            checker.IsPrime(4).Should().Be(false);
        }

        public IPrimeChecker CreatePrimeCheckerV5()
        {
            var t = IL.NewType().Implements<IPrimeChecker>()
                .WithMethod("IsPrime")
                .WithVariable<int>("i")
                .WithParameter<int>("number")
                .Returns<bool>()
                    .If("number<=1", @then: m=>m
                        .Ret(false)
                    )
                    .For("i", 2, "number/2", @do: m => m
                        .If("(number%i)==0", @then: b => b
                            .Ret(false)
                        )
                    )
                    .Ret(true)
                .AsType;

            return (IPrimeChecker)Activator.CreateInstance(t);
        }

        [Test]
        public void IsPrimeV5_PassingOk()
        {
            var checker = CreatePrimeCheckerV5();
            checker.IsPrime(0).Should().Be(false);
            checker.IsPrime(1).Should().Be(false);
            checker.IsPrime(2).Should().Be(true);
            checker.IsPrime(3).Should().Be(true);
            checker.IsPrime(4).Should().Be(false);
        }

        public IPrimeChecker CreatePrimeCheckerV6()
        {
            var t = IL.NewType().Implements<IPrimeChecker>()
                .WithMethod("IsPrime")
                .WithVariable<int>("i")
                .WithParameter<int>("number")
                .Returns<bool>()
                    .If("number<=1", @then: m => m
                        .Ret(false)
                    )
                    .Stloc(2, "i")
                    .While("i <= number/2", @do: m => m
                        .If("(number%i)==0", @then: b => b
                            .Ret(false)
                        )
                        .Inc("i")
                    )
                    .Ret(true)
                .AsType;

            return (IPrimeChecker)Activator.CreateInstance(t);
        }

        [Test]
        public void IsPrimeV6_PassingOk()
        {
            var checker = CreatePrimeCheckerV6();
            checker.IsPrime(0).Should().Be(false);
            checker.IsPrime(1).Should().Be(false);
            checker.IsPrime(2).Should().Be(true);
            checker.IsPrime(3).Should().Be(true);
            checker.IsPrime(4).Should().Be(false);
        }

        public IPrimeChecker CreatePrimeCheckerV7()
        {
            var t = IL.NewType().Implements<IPrimeChecker>()
                .WithMethod("IsPrime")
                .WithVariable<int>("i")
                .WithParameter<int>("number")
                .Returns<bool>()
                    .If("number<=1", @then: m => m
                        .Ret(false)
                    )
                    .Stloc(2, "i")
                    .Until("i > number/2", @do: m => m
                        .If("(number%i)==0", @then: b => b
                            .Ret(false)
                        )
                        .Inc("i")
                    )
                    .Ret(true)
                .AsType;

            return (IPrimeChecker)Activator.CreateInstance(t);
        }

        [Test]
        public void IsPrimeV7_PassingOk()
        {
            var checker = CreatePrimeCheckerV7();
            checker.IsPrime(0).Should().Be(false);
            checker.IsPrime(1).Should().Be(false);
            checker.IsPrime(2).Should().Be(true);
            checker.IsPrime(3).Should().Be(true);
            checker.IsPrime(4).Should().Be(false);
        }
    }

    public interface IPrimeChecker
    {
        bool IsPrime(int number);
    }
}
