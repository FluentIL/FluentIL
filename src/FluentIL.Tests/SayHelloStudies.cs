using System;
using System.Reflection.Emit;
using NUnit.Framework;
using SharpTestsEx;

namespace FluentIL.Tests
{
    /*
public string SayHello(string a)
{
    if (a == null)
        throw new ArgumentNullException("a");

    if (a == String.Empty)
        throw new ArgumentException("Argument 'a' cannot be empty");

    return ("Hello " + a);
}
     */

    /*
.method public hidebysig instance string 
        SayHello(string a) cil managed
{
    // Code size       50 (0x32)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000e
    IL_0003:  ldstr      "a"
    IL_0008:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
    IL_000d:  throw
    IL_000e:  ldarg.1
    IL_000f:  ldsfld     string [mscorlib]System.String::Empty
    IL_0014:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                    string)
    IL_0019:  brfalse.s  IL_0026
    IL_001b:  ldstr      "Argument 'a' cannot be empty"
    IL_0020:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string)
    IL_0025:  throw
    IL_0026:  ldstr      "Hello "
    IL_002b:  ldarg.1
    IL_002c:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0031:  ret
} // end of method Program::SayHello
    */

    [TestFixture]
    public class SayHelloStudies
    {
        private ISayHello2 CreateSayHelloV1()
        {
            var argumentNullExceptionConstructor = typeof(ArgumentNullException)
                .GetConstructor(new[] { typeof(string) });

            argumentNullExceptionConstructor.Should().Not.Be(null);

            var argumentExceptionConstructor = typeof(ArgumentException)
                .GetConstructor(new[] { typeof(string) });

            var stringEmptyField = typeof(string).GetField("Empty");

            var stringOp_EqualityMethod = typeof(string).GetMethod("op_Equality",
                new[] { typeof(string), typeof(string) }
                );

            var stringConcatMethod = typeof(string).GetMethod("Concat",
                new[] { typeof(string), typeof(string) });
            stringConcatMethod.Should().Not.Be(null);

            var t = IL.NewType().Implements<ISayHello2>()
                .WithMethod("SayHello")
                    .WithParameter(typeof(string), "a")
                    .Returns(typeof(string))

                    .Ldarg("a")
                    .Brtrue_S("NotNull")
                    .Ldstr("a")
                    .Emit(OpCodes.Newobj, argumentNullExceptionConstructor)
                    .Emit(OpCodes.Throw)
                    .MarkLabel("NotNull")

                    .Ldarg("a")
                    .Emit(OpCodes.Ldsfld, stringEmptyField)
                    .Emit(OpCodes.Call, stringOp_EqualityMethod)
                    .Brfalse_S("NotEmpty")

                    .Ldstr("Argument 'a' cannot be empty")
                    .Emit(OpCodes.Newobj, argumentExceptionConstructor)
                    .Emit(OpCodes.Throw)

                    .MarkLabel("NotEmpty")
                    .Ldstr("Hello ")
                    .Ldarg("a")
                    .Emit(OpCodes.Call, stringConcatMethod)
                    .Ret()
                .AsType;

            return (ISayHello2)Activator.CreateInstance(t);
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SayHelloV1_PassingNull_ThrowsArgumentNullException()
        {
            this.CreateSayHelloV1().SayHello(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SayHelloV1_PassingEmpty_ThrowsArgumentException()
        {
            this.CreateSayHelloV1().SayHello("");
        }

        [Test]
        public void SayHelloV1_PassingElemar_ReturnsHelloElemar()
        {
            var result = this.CreateSayHelloV1().SayHello("Elemar");
            result.Should().Be("Hello Elemar");
        }


        public ISayHello CheckingEmptyParameterV1()
        {
            var argumentExceptionConstructor = typeof(ArgumentException)
                .GetConstructor(new[] { typeof(string) });
            var stringEmptyField = typeof(string).GetField("Empty");
            var stringOp_EqualityMethod = typeof(string).GetMethod("op_Equality", new[] { typeof(string), typeof(string) });

            var t = IL.NewType()
                .Implements<ISayHello>()
                .WithMethod("SayHello")
                    .WithParameter(typeof(string), "a")
                    .Returns(typeof(void))
                    .Ldarg("a")
                    .Emit(OpCodes.Ldsfld, stringEmptyField)
                    .Emit(OpCodes.Call, stringOp_EqualityMethod)
                    .Brfalse_S("NotEmpty")

                    .Ldstr("Argument 'a' cannot be empty")
                    .Emit(OpCodes.Newobj, argumentExceptionConstructor)
                    .Emit(OpCodes.Throw)

                    .MarkLabel("NotEmpty")
                    .Ret()
                .AsType;

            return (ISayHello)Activator.CreateInstance(t);

        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckingEmptyParameterV1_PassingEmpty_ThrowsArgumentException()
        {

            CheckingEmptyParameterV1().SayHello(String.Empty);
        }

        [Test]
        public void CheckingEmptyParameterV1_PassingNotEmpty_ThrowsNothing()
        {
            CheckingEmptyParameterV1().SayHello("Elemar");
        }


        public ISayHello CheckingEmptyParameterV2()
        {
            var argumentExceptionConstructor = typeof(ArgumentException)
                .GetConstructor(new[] { typeof(string) });

            var t = IL.NewType()
                .Implements<ISayHello>()
                .WithMethod("SayHello")
                    .WithParameter(typeof(string), "a")
                    .Returns(typeof(void))
                    .Ldarg("a")
                    .IfEmptyString()
                        .Ldstr("Argument 'a' cannot be empty")
                        .Newobj(argumentExceptionConstructor)
                        .Throw()
                    .EndIf()
                    .Ret()
                .AsType;

            return (ISayHello)Activator.CreateInstance(t);

        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckingEmptyParameterV2_PassingEmpty_ThrowsArgumentException()
        {

            CheckingEmptyParameterV2().SayHello(String.Empty);
        }

        [Test]
        public void CheckingEmptyParameterV2_PassingNotEmpty_ThrowsNothing()
        {
            CheckingEmptyParameterV2().SayHello("Elemar");
        }

        public ISayHello CheckingEmptyParameterV3()
        {
            var argumentExceptionConstructor = typeof(ArgumentException)
                .GetConstructor(new[] { typeof(string) });

            var t = IL.NewType()
                .Implements<ISayHello>()
                .WithMethod("SayHello")
                    .WithParameter(typeof(string), "a")
                    .Returns(typeof(void))
                    .Ldarg("a")
                    .IfEmptyString(@then: m=>m
                        .Ldstr("Argument 'a' cannot be empty")
                        .Newobj(argumentExceptionConstructor)
                        .Throw()
                    )
                    .Ret()
                .AsType;

            return (ISayHello)Activator.CreateInstance(t);

        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckingEmptyParameterV3_PassingEmpty_ThrowsArgumentException()
        {

            CheckingEmptyParameterV2().SayHello(string.Empty);
        }

        [Test]
        public void CheckingEmptyParameterV3_PassingNotEmpty_ThrowsNothing()
        {
            CheckingEmptyParameterV2().SayHello("Elemar");
        }

        public ISayHello CheckingNullParameterV1()
        {
            var argumentNullExceptionConstructor = typeof(ArgumentNullException)
                .GetConstructor(new[] { typeof(string) });

            var t = IL.NewType()
                .Implements<ISayHello>()
                .WithMethod("SayHello")
                    .WithParameter(typeof(string), "a")
                    .Returns(typeof(void))
                    .Ldarg("a")
                    .Brtrue_S("NotNull")
                    .Ldstr("a")
                    .Emit(OpCodes.Newobj, argumentNullExceptionConstructor)
                    .Emit(OpCodes.Throw)
                    .MarkLabel("NotNull")
                    .Ret()
                .AsType;

            return (ISayHello)Activator.CreateInstance(t);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckingNullParameterV1_PassingNull_ThrowsArgumentNullException()
        {
            CheckingNullParameterV1().SayHello(null);
        }

        [Test]
        public void CheckingNullParameterV1_PassingNotNull_ThrowsNothing()
        {
            CheckingNullParameterV1().SayHello("Elemar");
        }


        public ISayHello CheckingNullParameterV2()
        {
            var argumentNullExceptionConstructor = typeof(ArgumentNullException)
                .GetConstructor(new[] { typeof(string) });

            var t = IL.NewType()
                .Implements<ISayHello>()
                .WithMethod("SayHello")
                    .WithParameter(typeof(string), "a")
                    .Returns(typeof(void))
                    .Ldarg("a")
                    .IfNull()
                        .Ldstr("a")
                        .Newobj(argumentNullExceptionConstructor)
                        .Throw()
                    .EndIf()
                    .Ret()
                .AsType;

            return (ISayHello)Activator.CreateInstance(t);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckingNullParameterV2_PassingNull_ThrowsArgumentNullException()
        {
            CheckingNullParameterV2().SayHello(null);
        }

        [Test]
        public void CheckingNullParameterV2_PassingNotNull_ThrowsNothing()
        {
            CheckingNullParameterV2().SayHello("Elemar");
        }

        public ISayHello CheckingNullParameterV3()
        {
            var argumentNullExceptionConstructor = typeof(ArgumentNullException)
                .GetConstructor(new[] { typeof(string) });

            var t = IL.NewType()
                .Implements<ISayHello>()
                .WithMethod("SayHello")
                    .WithParameter<string>("a")
                    .Returns(typeof(void))
                    .Ldarg("a")
                    .IfNull( @then: m => m
                        .Ldstr("a")
                        .Newobj(argumentNullExceptionConstructor)
                        .Throw()
                    )
                    .Ret()
                .AsType;

            return (ISayHello)Activator.CreateInstance(t);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckingNullParameterV3_PassingNull_ThrowsArgumentNullException()
        {
            CheckingNullParameterV3().SayHello(null);
        }

        [Test]
        public void CheckingNullParameterV3_PassingNotNull_ThrowsNothing()
        {
            CheckingNullParameterV3().SayHello("Elemar");
        }


        private ISayHello2 CreateSayHelloV2()
        {
            var argumentNullExceptionConstructor = typeof(ArgumentNullException)
                .GetConstructor(new[] { typeof(string) });

            argumentNullExceptionConstructor.Should().Not.Be(null);

            var argumentExceptionConstructor = typeof(ArgumentException)
                .GetConstructor(new[] { typeof(string) });


            var stringEmptyField = typeof(string).GetField("Empty");

            var stringOp_EqualityMethod = typeof(string).GetMethod("op_Equality",
                new[] { typeof(string), typeof(string) });

            var stringConcatMethod = typeof(string).GetMethod("Concat",
                new[] { typeof(string), typeof(string) });
            stringConcatMethod.Should().Not.Be(null);

            var t = IL.NewType().Implements<ISayHello2>()
                .WithMethod("SayHello")
                    .WithParameter(typeof(string), "a")
                    .Returns(typeof(string))

                    .Ldarg("a")
                    .Brtrue_S("NotNull")
                    .Ldstr("a")
                    .Newobj(argumentNullExceptionConstructor)
                    .Throw()
                    .MarkLabel("NotNull")

                    .Ldarg("a")
                    .Ldsfld(stringEmptyField)
                    .Call(stringOp_EqualityMethod)
                    .Brfalse_S("NotEmpty")

                    .Ldstr("Argument 'a' cannot be empty")
                    .Newobj(argumentExceptionConstructor)
                    .Throw()

                    .MarkLabel("NotEmpty")
                    .Ldstr("Hello ")
                    .Ldarg("a")
                    .Call(stringConcatMethod)

                    .Ret()
                .AsType;

            return (ISayHello2)Activator.CreateInstance(t);
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SayHelloV2_PassingNull_ThrowsArgumentNullException()
        {
            this.CreateSayHelloV2().SayHello(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SayHelloV2_PassingEmpty_ThrowsArgumentException()
        {
            this.CreateSayHelloV2().SayHello("");
        }

        [Test]
        public void SayHelloV2_PassingElemar_ReturnsHelloElemar()
        {
            var result = this.CreateSayHelloV2().SayHello("Elemar");
            result.Should().Be("Hello Elemar");
        }

        private ISayHello2 CreateSayHelloV3()
        {
            var argumentNullExceptionConstructor = typeof(ArgumentNullException)
                .GetConstructor(new[] { typeof(string) });

            argumentNullExceptionConstructor.Should().Not.Be(null);

            var argumentExceptionConstructor = typeof(ArgumentException)
                .GetConstructor(new[] { typeof(string) });

            var stringConcatMethod = typeof(string).GetMethod("Concat",
                new[] { typeof(string), typeof(string) });
            stringConcatMethod.Should().Not.Be(null);

            var t = IL.NewType().Implements<ISayHello2>()
                .WithMethod("SayHello")
                    .WithParameter(typeof(string), "a")
                    .Returns(typeof(string))

                    .Ldarg("a")
                    .IfNull()
                        .Ldstr("a")
                        .Newobj(argumentNullExceptionConstructor)
                        .Throw()
                    .EndIf()

                    .Ldarg("a")
                    .IfEmptyString()
                        .Ldstr("Argument 'a' cannot be empty")
                        .Newobj(argumentExceptionConstructor)
                        .Throw()
                    .EndIf()

                    .Ldstr("Hello ")
                    .Ldarg("a")
                    .Call(stringConcatMethod)

                    .Ret()
                .AsType;

            return (ISayHello2)Activator.CreateInstance(t);
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SayHelloV3_PassingNull_ThrowsArgumentNullException()
        {
            this.CreateSayHelloV3().SayHello(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SayHelloV3_PassingEmpty_ThrowsArgumentException()
        {
            this.CreateSayHelloV3().SayHello("");
        }

        [Test]
        public void SayHelloV3_PassingElemar_ReturnsHelloElemar()
        {
            var result = this.CreateSayHelloV3().SayHello("Elemar");
            result.Should().Be("Hello Elemar");
        }


        private ISayHello2 CreateSayHelloV4()
        {
            var stringConcatMethod = typeof(string).GetMethod("Concat",
                new[] { typeof(string), typeof(string) });

            var t = IL.NewType().Implements<ISayHello2>()
                .WithMethod("SayHello")
                    .WithParameter(typeof(string), "a")
                    .Returns(typeof(string))

                    .Ldarg("a")
                    .IfNull()
                        .Ldstr("a")
                        .Newobj<ArgumentNullException>(typeof(string))
                        .Throw()
                    .EndIf()

                    .Ldarg("a")
                    .IfEmptyString()
                        .Ldstr("Argument 'a' cannot be empty")
                        .Newobj<ArgumentException>(typeof(string))
                        .Throw()
                    .EndIf()

                    .Ldstr("Hello ")
                    .Ldarg("a")
                    .Call(stringConcatMethod)

                    .Ret()
                .AsType;

            return (ISayHello2)Activator.CreateInstance(t);
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SayHelloV4_PassingNull_ThrowsArgumentNullException()
        {
            this.CreateSayHelloV4().SayHello(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SayHelloV4_PassingEmpty_ThrowsArgumentException()
        {
            this.CreateSayHelloV4().SayHello("");
        }

        [Test]
        public void SayHelloV4_PassingElemar_ReturnsHelloElemar()
        {
            var result = this.CreateSayHelloV4().SayHello("Elemar");
            result.Should().Be("Hello Elemar");
        }


        private ISayHello2 CreateSayHelloV5()
        {
            var t = IL.NewType().Implements<ISayHello2>()
                .WithMethod("SayHello")
                    .WithParameter(typeof(string), "a")
                    .Returns(typeof(string))

                    .Ldarg("a")
                    .IfNull()
                        .Ldstr("a")
                        .Throw<ArgumentNullException>(typeof(string))
                    .EndIf()

                    .Ldarg("a")
                    .IfEmptyString()
                        .Ldstr("Argument 'a' cannot be empty")
                        .Throw<ArgumentException>(typeof(string))
                    .EndIf()

                    .Ldstr("Hello ")
                    .Ldarg("a")
                    .Call<string>("Concat", typeof(string), typeof(string))

                    .Ret()
                .AsType;

            return (ISayHello2)Activator.CreateInstance(t);
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SayHelloV5_PassingNull_ThrowsArgumentNullException()
        {
            this.CreateSayHelloV5().SayHello(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SayHelloV5_PassingEmpty_ThrowsArgumentException()
        {
            this.CreateSayHelloV5().SayHello("");
        }

        [Test]
        public void SayHelloV5_PassingElemar_ReturnsHelloElemar()
        {
            var result = this.CreateSayHelloV5().SayHello("Elemar");
            result.Should().Be("Hello Elemar");
        }

    }

    public interface ISayHello
    {
        void SayHello(string a);
    }

    public interface ISayHello2
    {
        string SayHello(string a);
    }
}
