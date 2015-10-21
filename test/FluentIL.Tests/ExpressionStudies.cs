using System.Linq.Expressions;
using NUnit.Framework;
using SharpTestsEx;

namespace FluentIL.Tests
{
    [TestFixture]
    public class ExpressionStudies
    {
        [Test]
        public void TwoPlusTwo_ResultsFour()
        {
            var method = IL.NewMethod()
                .Returns(typeof(int))
                .Expression(
                    Expression.Add(
                        Expression.Constant(2),
                        Expression.Constant(2)
                    )
                )
                .Ret();

            method.Invoke().Should().Be(4);
        }

        [Test]
        public void TwoTimesThree_ResultsSix()
        {
            var method = IL.NewMethod()
                .Returns(typeof(int))
                .Expression(
                    Expression.Multiply(
                        Expression.Constant(2),
                        Expression.Constant(3)
                    )
                )
                .Ret();

            method.Invoke().Should().Be(6);
        }

        [Test]
        public void TwoPlusResultOfTwoTimesThree_ResultsEight()
        {
            var method = IL.NewMethod()
                .Returns(typeof(int))
                .Expression(
                    Expression.Add(
                        Expression.Constant(2),
                        Expression.Multiply(
                            Expression.Constant(2),
                            Expression.Constant(3)
                            )
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(8);
        }

        [Test]
        public void Divide100By5_Results20()
        {
            var method = IL.NewMethod()
                .Returns(typeof(int))
                .Expression(
                    Expression.Divide(
                        Expression.Constant(100),
                        Expression.Constant(5)
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(20);
        }

        [Test]
        public void EightSubtractThree_ResultsFive()
        {
            var method = IL.NewMethod()
                .Returns(typeof(int))
                .Expression(
                    Expression.Subtract(
                        Expression.Constant(8),
                        Expression.Constant(3)
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(5);
        }

        [Test]
        public void FiveDividedByTwo_ResultsTwo()
        {
            var method = IL.NewMethod()
                .Returns(typeof(int))
                .Expression(
                    Expression.Divide(
                        Expression.Constant(5),
                        Expression.Constant(2)
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(2);
        }

        [Test]
        public void DoubleFiveDividedByDoubleTwo_ResultsTwoPointFive()
        {
            var method = IL.NewMethod()
                .Returns(typeof(double))
                .Expression(
                    Expression.Divide(
                        Expression.Constant(5.0),
                        Expression.Constant(2.0)
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(2.5);
        }

        [Test]
        public void Paramenter5DividedBy2_ResultsTwoPointFive()
        {
            var method = IL.NewMethod()
                .WithParameter(typeof(double), "a")
                .Returns(typeof(double))
                .Expression(
                    Expression.Divide(
                        Expression.Parameter(typeof(double), "a"),
                        Expression.Constant(2.0)
                        )
                    )
                .Ret();

            method.Invoke(5.0).Should().Be(2.5);
        }

        [Test]
        public void EnsureLimits()
        {
            var method = IL.NewMethod()
                .WithParameter(typeof(int), "value")
                .Returns(typeof(int))
                .LdLocOrArg("value")
                .EnsureLimits(
                    Expression.Constant(5),
                    Expression.Constant(10)
                )
                .Ret();

            method.Invoke(4).Should().Be(5);
        }

        [Test]
        public void GreaterThanOrEqual_Passing2And2_ReturnsTrue()
        {
            var method = IL.NewMethod()
                .Returns(typeof(bool))
                .Expression(
                    Expression.GreaterThanOrEqual(
                        Expression.Constant(2),
                        Expression.Constant(2)
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(true);
        }

        [Test]
        public void GreaterThanOrEqual_Passing3And2_ReturnsTrue()
        {
            var method = IL.NewMethod()
                .Returns(typeof(bool))
                .Expression(
                    Expression.GreaterThanOrEqual(
                        Expression.Constant(3),
                        Expression.Constant(2)
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(true);
        }

        [Test]
        public void GreaterThanOrEqual_Passing2And3_ReturnsFalse()
        {
            var method = IL.NewMethod()
                .Returns(typeof(bool))
                .Expression(
                    Expression.GreaterThanOrEqual(
                        Expression.Constant(2),
                        Expression.Constant(3)
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(false);
        }

        [Test]
        public void LessThanOrEqual_Passing2And2_ReturnsTrue()
        {
            var method = IL.NewMethod()
                .Returns(typeof(bool))
                .Expression(
                    Expression.LessThanOrEqual(
                        Expression.Constant(2),
                        Expression.Constant(2)
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(true);
        }

        [Test]
        public void Equal_Passing2And2_ReturnsTrue()
        {
            var method = IL.NewMethod()
                .Returns(typeof(bool))
                .Expression(
                    Expression.Equal(
                        Expression.Constant(2),
                        Expression.Constant(2)
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(true);
        }

        [Test]
        public void Not_Equal_Passing2And2_ReturnsFalse()
        {
            var method = IL.NewMethod()
                .Returns(typeof(bool))
                .Expression(
                    Expression.Not(
                        Expression.Equal(
                            Expression.Constant(2),
                            Expression.Constant(2)
                            )
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(false);
        }

        [Test]
        public void LessThanOrEqual_Passing3And2_ReturnsFalse()
        {
            var method = IL.NewMethod()
                .Returns(typeof(bool))
                .Expression(
                    Expression.LessThanOrEqual(
                        Expression.Constant(3),
                        Expression.Constant(2)
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(false);
        }

        [Test]
        public void LessThanOrEqual_Passing2And3_ReturnsTrue()
        {
            var method = IL.NewMethod()
                .Returns(typeof(bool))
                .Expression(
                    Expression.LessThanOrEqual(
                        Expression.Constant(2),
                        Expression.Constant(3)
                        )
                    )
                .Ret();

            method.Invoke().Should().Be(true);
        }

        [Test]
        public void ConditionAnd()
        {
            var method = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .Returns(typeof(bool))
                .Expression(
                    Expression.AndAlso(
                        Expression.GreaterThan(
                                Expression.Parameter(typeof(int), "a"),
                                Expression.Constant(5)
                                ),
                        Expression.LessThan(
                                Expression.Parameter(typeof(int), "a"),
                                Expression.Constant(10)
                                )
                        )
                    )
                .Ret();

            method.Invoke(5).Should().Be(false);
            method.Invoke(7).Should().Be(true);
            method.Invoke(10).Should().Be(false);
            method.Invoke(11).Should().Be(false);
        }

        [Test]
        public void ConditionOr()
        {
            var method = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .Returns(typeof(bool))
                .Expression(
                    Expression.OrElse(
                        Expression.AndAlso(
                            Expression.GreaterThan(
                                    Expression.Parameter(typeof(int), "a"),
                                    Expression.Constant(5)
                                    ),
                            Expression.LessThan(
                                    Expression.Parameter(typeof(int), "a"),
                                    Expression.Constant(10)
                                    )
                            )
                        ,
                        Expression.AndAlso(
                            Expression.GreaterThan(
                                    Expression.Parameter(typeof(int), "a"),
                                    Expression.Constant(15)
                                    ),
                            Expression.LessThan(
                                    Expression.Parameter(typeof(int), "a"),
                                    Expression.Constant(20)
                                    )
                            )
                        )
                    )
                .Ret();

            method.Invoke(5).Should().Be(false);
            method.Invoke(7).Should().Be(true);
            method.Invoke(10).Should().Be(false);
            method.Invoke(11).Should().Be(false);
            method.Invoke(17).Should().Be(true);
        }

    }
}
