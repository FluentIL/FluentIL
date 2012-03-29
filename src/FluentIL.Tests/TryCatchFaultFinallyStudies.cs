using System;

using NUnit.Framework;
using SharpTestsEx;

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
    }
}
