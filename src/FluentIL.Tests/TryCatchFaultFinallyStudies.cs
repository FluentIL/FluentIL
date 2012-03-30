using System;

using NUnit.Framework;
using SharpTestsEx;
using FluentIL.Emitters;

namespace FluentIL.Tests
{
    [TestFixture]
    class TryCatchFaultFinallyStudies
    {
        [Test]
        public void TryCatch1()
        {
            var method = IL.NewMethod()
                .WithParameter<int>("value")
                .WithVariable<int>("result")
                .Returns<int>()
                .Try(m => m
                    .If("value > 20", mif => mif
                        .Throw<ArgumentException>()
                    )
                    .If("value > 10", mif => mif
                        .Throw<Exception>()
                    )
                    .Ldarg("value")
                    .Stloc("result")
                    ,
                    IL.Catch<ArgumentException>(mex => mex
                        .Pop()
                        .Stloc(-2, "result")
                    )
                    ,
                    IL.Catch<Exception>(mex => mex
                        .Pop()
                        .Stloc(-1, "result")
                    )
                )
                .Ldloc("result")
                .Ret();

            method.Invoke(11).Should().Be(-1);
            method.Invoke(21).Should().Be(-2);
            method.Invoke(5).Should().Be(5);
        }

        [Test]
        public void TryCatchFinally1()
        {
            Action<DynamicMethodBody> tryBody = m => m
                .If("value > 20", mif => mif
                    .Throw<ArgumentException>()
                )
                .If("value > 10", mif => mif
                    .Throw<Exception>()
                )
                .Ldarg("value")
                .Stloc("result");

            var catchArgumentExceptionBody = IL.Catch<ArgumentException>(mex => mex
                    .Pop()
                    .Stloc(-2, "result")
                );

            var catchExceptionBody = IL.Catch<Exception>(mex => mex
                    .Pop()
                    .Stloc(-1, "result")
                );

            Action<DynamicMethodBody> finallyBody = m => m
                .Ldarg("log")
                .Ldc(1)
                .CallSet<ExceptionLog>("FinallyWasExecuted");

            var method = IL.NewMethod()
                .WithParameter<int>("value")
                .WithParameter<ExceptionLog>("log")
                .WithVariable<int>("result")
                .Returns<int>()
                .Try(
                    tryBody,
                    finallyBody,
                    catchArgumentExceptionBody,
                    catchExceptionBody
                )
                .Ldloc("result")
                .Ret();

            var log = new ExceptionLog();
            method.Invoke(11, log).Should().Be(-1);
            log.FinallyWasExecuted.Should().Be(true);
            log = new ExceptionLog();
            method.Invoke(21, log).Should().Be(-2);
            log.FinallyWasExecuted.Should().Be(true);
            log = new ExceptionLog();
            method.Invoke(5, log).Should().Be(5);
            log.FinallyWasExecuted.Should().Be(true);
        }
    }

    public class ExceptionLog
    {
        public bool FinallyWasExecuted { get; set; }
    }
}
