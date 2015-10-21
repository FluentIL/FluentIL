using System;
using System.Linq;
using FluentIL.ExpressionParser;
using NUnit.Framework;
using SharpTestsEx;

namespace FluentIL.Tests
{
    [TestFixture]
    public class ExpressionScannerTests
    {
        [Test]
        public void Expression1()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan("2+2/2");

            result.Should().Have.SameSequenceAs(
                new Token("integer", "2"),
                new Token("plus", "+"),
                new Token("integer", "2"),
                new Token("divide", "/"),
                new Token("integer", "2")
                );
        }

        [Test]
        public void Expression2()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan("22+2/2");

            result.Should().Have.SameSequenceAs(
                new Token("integer", "22"),
                new Token("plus", "+"),
                new Token("integer", "2"),
                new Token("divide", "/"),
                new Token("integer", "2")
                );
        }

        [Test]
        public void Expression3()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan("2.2+2/2");

            result.Should().Have.SameSequenceAs(
                new Token("float", "2.2"),
                new Token("plus", "+"),
                new Token("integer", "2"),
                new Token("divide", "/"),
                new Token("integer", "2")
                );
        }

        [Test]
        public void Expression4()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan("2.2.3");

            result.Should().Have.SameSequenceAs(
                new Token("float", "2.2"),
                new Token("float", ".3")
                );
        }


        [Test]
        public void Expression5()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan("(2.5+4)");

            result.Should().Have.SameSequenceAs(
                new Token("lparen", "("),
                new Token("float", "2.5"),
                new Token("plus", "+"),
                new Token("integer", "4"),
                new Token("rparen", ")")
                );
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Expression6()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan("var=10").ToArray();
        }

        [Test]
        public void Expression7()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan("var:=10");

            result.Should().Have.SameSequenceAs(
                new Token("identifier", "var"),
                new Token("assign", ":="),
                new Token("integer", "10")
                );
        }

        [Test]
        public void Expression8()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan("var*10");

            result.Should().Have.SameSequenceAs(
                new Token("identifier", "var"),
                new Token("times", "*"),
                new Token("integer", "10")
                );
        }

        [Test]
        public void Expression9()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan("var* /* comment */ 10");

            result.Should().Have.SameSequenceAs(
                new Token("identifier", "var"),
                new Token("times", "*"),
                new Token("white_space", " "),
                new Token("comment", "/* comment */"),
                new Token("white_space", " "),
                new Token("integer", "10")
                );
        }

        [Test]
        public void Expression10()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan(">");

            result.Should().Have.SameSequenceAs(
                new Token("gt", ">")
                );
        }
        
        [Test]
        public void Expression11()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan("==");

            result.Should().Have.SameSequenceAs(
                new Token("eq", "==")
                );
        }

        [Test]
        public void Expression12()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan(">=");

            result.Should().Have.SameSequenceAs(
                new Token("geq", ">=")
                );
        }
        
        [Test]
        public void Expression13()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan(">>=");

            result.Should().Have.SameSequenceAs(
                new Token("gt", ">"),
                new Token("geq", ">=")
                );
        }

        [Test]
        public void Expression14()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan(">>=<");

            result.Should().Have.SameSequenceAs(
                new Token("gt", ">"),
                new Token("geq", ">="),
                new Token("lt", "<")
                );
        }

        [Test]
        public void Expression15()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan(">>=<<=");

            result.Should().Have.SameSequenceAs(
                new Token("gt", ">"),
                new Token("geq", ">="),
                new Token("lt", "<"),
                new Token("leq", "<=")
                );
        }

        [Test]
        public void Expression16()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan(">>=<<===");

            result.Should().Have.SameSequenceAs(
                new Token("gt", ">"),
                new Token("geq", ">="),
                new Token("lt", "<"),
                new Token("leq", "<="),
                new Token("eq", "==")
                );
        }


        [Test]
        public void Expression17()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan(">>=<<===!!=");

            result.Should().Have.SameSequenceAs(
                new Token("gt", ">"),
                new Token("geq", ">="),
                new Token("lt", "<"),
                new Token("leq", "<="),
                new Token("eq", "=="),
                new Token("not", "!"),
                new Token("neq", "!=")
                );
        }

        [Test]
        public void Expression18()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan(">>=<><<===!!=");

            result.Should().Have.SameSequenceAs(
                new Token("gt", ">"),
                new Token("geq", ">="),
                new Token("neq", "<>"),
                new Token("lt", "<"),
                new Token("leq", "<="),
                new Token("eq", "=="),
                new Token("not", "!"),
                new Token("neq", "!=")
                );
        }
        
        [Test]
        public void Expression19()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan(">>=<>||&&<<===!!=");

            result.Should().Have.SameSequenceAs(
                new Token("gt", ">"),
                new Token("geq", ">="),
                new Token("neq", "<>"),
                new Token("or", "||"),
                new Token("and", "&&"),
                new Token("lt", "<"),
                new Token("leq", "<="),
                new Token("eq", "=="),
                new Token("not", "!"),
                new Token("neq", "!=")
                );
        }

        [Test]
        public void Expression20()
        {
            var scanner = new ExpressionScanner();
            var result = scanner.Scan("^>>=<>||&&<<===!!=");

            result.Should().Have.SameSequenceAs(
                new Token("pow", "^"),
                new Token("gt", ">"),
                new Token("geq", ">="),
                new Token("neq", "<>"),
                new Token("or", "||"),
                new Token("and", "&&"),
                new Token("lt", "<"),
                new Token("leq", "<="),
                new Token("eq", "=="),
                new Token("not", "!"),
                new Token("neq", "!=")
                );
        }
    }
}
