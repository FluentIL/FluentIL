using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentIL.Tests
{
    using NUnit.Framework;
    using FluentIL.ExpressionParser;

    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void ExpressionParse01()
        {
            Parser.Parse("2");
        }

        [Test]
        public void ExpressionParse02()
        {
            Parser.Parse("2+2");
        }

        [Test]
        public void ExpressionParse03()
        {
            Parser.Parse("-3");
        }
        
        [Test]
        public void ExpressionParse04()
        {
            Parser.Parse("2+2/2");
        }
        
        [Test]
        public void ExpressionParse05()
        {
            Parser.Parse("(2+2)/2");
        }

        [Test]
        public void ExpressionParse06()
        {
            Parser.Parse("3>2");
        }

        [Test]
        public void ExpressionParse07()
        {
            Parser.Parse("var>2");
        }

        [Test]
        public void ExpressionParse08()
        {
            Parser.Parse("var<=(2+2/2)");
        }

        [Test]
        public void ExpressionParse09()
        {
            Parser.Parse("4==3");
        }

        [Test]
        public void ExpressionParse10()
        {
            Parser.Parse("!(4==3)");
        }

        [Test]
        public void ExpressionParse11()
        {
            Parser.Parse("!(a>b)||(a==b)");
        }

        [Test]
        public void ExpressionParse12()
        {
            Parser.Parse("!(a>b)&&(a==b)");
        }

        [Test]
        public void ExpressionParse13()
        {
            Parser.Parse("!(a> b)&&(a==b)");
        }

        [Test]
        public void ExpressionParse14()
        {
            Parser.Parse("!(a> b)&& /* comment */ (a==b)");
        }

        
    }
}
