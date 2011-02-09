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
    public class ConditionalStudies
    {
        [Test]
        public void TwoNumbersAreEquals_TrueReturnsYesFalseReturnsNo_Reference()
        {
            var dm = IL.NewMethod(typeof(string), typeof(int), typeof(int))
                .Ldarg(0, 1)
                .Beq("if_true")
                .Ldstr("No")
                .Br("done")
                .MarkLabel("if_true")
                .Ldstr("Yes")
                .MarkLabel("done")
                .Ret();

            dm.Invoke(2, 2).Should().Be("Yes");
            dm.Invoke(2, 3).Should().Be("No");
        }

        [Test]
        public void TwoNumbersAreEquals_TrueReturnsYesFalseReturnsNo_Reference2()
        {
            var dm = IL.NewMethod(typeof(string), typeof(int), typeof(int))
                .Ldarg(0, 1)
                .Ceq()
                .Brfalse("if_false")
                .Ldstr("Yes")
                .Br("done")
                .MarkLabel("if_false")
                .Ldstr("No")
                .MarkLabel("done")
                .Ret();

            dm.Invoke(2, 2).Should().Be("Yes");
            dm.Invoke(2, 3).Should().Be("No");
        }

        [Test]
        public void TwoNumbersAreEquals_TrueReturnsYesFalseReturnsNo()
        {
            var dm = IL.NewMethod(typeof(string), typeof(int), typeof(int))
                .Ldarg(0, 1)
                .Ifeq()
                    .Ldstr("Yes")
                .Else()
                    .Ldstr("No")
                .EndIf()
                .Ret();

            dm.Invoke(2, 2).Should().Be("Yes");
            dm.Invoke(2, 3).Should().Be("No");
        }
    }
}
