using NUnit.Framework;
using SharpTestsEx;

namespace FluentIL.Tests
{
    [TestFixture]
    public class ExpressionParsingStudies
    {
        [Test]
        public void Parsing01()
        {
            var il = IL.NewMethod()
                .Returns(typeof(int))
                .Parse("2+2")
                .Ret();

            var result = il.Invoke();

            result.Should().Be(4);    
        }

        [Test]
        public void Parsing02()
        {
            var il = IL.NewMethod()
                .Returns(typeof(int))
                .Parse("2+2/2")
                .Ret();

            var result = il.Invoke();

            result.Should().Be(3);
        }

        [Test]
        public void Parsing03()
        {
            var il = IL.NewMethod()
                .Returns(typeof(int))
                .Parse("(2+2)/2")
                .Ret();

            var result = il.Invoke();

            result.Should().Be(2);
        }

        [Test]
        public void Parsing04()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .WithParameter(typeof(int), "b")
                .Returns(typeof(int))
                .Parse("a+b")
                .Ret();

            var result = il.Invoke(2, 3);

            result.Should().Be(5);
        }

        [Test]
        public void Parsing05()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .WithParameter(typeof(int), "b")
                .Returns(typeof(bool))
                .Parse("a<b")
                .Ret();

            var result = il.Invoke(2, 3);

            result.Should().Be(true);
        }

        [Test]
        public void Parsing06()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .WithParameter(typeof(int), "b")
                .Returns(typeof(bool))
                .Parse("a>b")
                .Ret();

            var result = il.Invoke(2, 3);

            result.Should().Be(false);
        }

        [Test]
        public void Parsing07()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .WithParameter(typeof(int), "b")
                .Returns(typeof(bool))
                .Parse("a>=b")
                .Ret();

            var result = il.Invoke(3, 3);

            result.Should().Be(true);
        }

        [Test]
        public void Parsing08()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .WithParameter(typeof(int), "b")
                .Returns(typeof(bool))
                .Parse("a<=b")
                .Ret();

            var result = il.Invoke(3, 3);

            result.Should().Be(true);
        }

        [Test]
        public void Parsing09()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .WithParameter(typeof(int), "b")
                .Returns(typeof(bool))
                .Parse("a==b")
                .Ret();

            var result = il.Invoke(3, 3);

            result.Should().Be(true);
        }

        [Test]
        public void Parsing10()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .WithParameter(typeof(int), "b")
                .Returns(typeof(bool))
                .Parse("a!=b")
                .Ret();

            var result = il.Invoke(3, 3);

            result.Should().Be(false);
        }

        [Test]
        public void Parsing11()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .WithParameter(typeof(int), "b")
                .Returns(typeof(bool))
                .Parse("!(a<=b)")
                .Ret();

            var result = il.Invoke(3, 3);

            result.Should().Be(false);
        }

        [Test]
        public void Parsing12()
        {
            var il = IL.NewMethod()
                .Returns(typeof(int))
                .Parse("-(2+8)")
                .Ret();

            var result = il.Invoke();

            result.Should().Be(-10);
        }

        [Test]
        public void Parsing13()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .Returns(typeof(bool))
                .Parse("(a>5)&&(a<10)")
                .Ret();

            il.Invoke(6).Should().Be(true) ;
            il.Invoke(11).Should().Be(false);
        }

        [Test]
        public void Parsing14()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .Returns(typeof(bool))
                .Parse("(a%2)==0")
                .Ret();

            il.Invoke(6).Should().Be(true);
            il.Invoke(5).Should().Be(false);
        }

        [Test]
        public void Parsing15()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .Returns(typeof(bool))
                .Parse("(a>5)&&(a<10)&&(a%2==0)")
                .Ret();
            
            il.Invoke(5).Should().Be(false);
            il.Invoke(6).Should().Be(true);
            il.Invoke(7).Should().Be(false);
            il.Invoke(8).Should().Be(true);
            il.Invoke(9).Should().Be(false);
            il.Invoke(10).Should().Be(false);
        }
        
        [Test]
        public void Parsing16()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .Returns(typeof(bool))
                .Parse("(a==6)||(a==8)")
                .Ret();

            il.Invoke(5).Should().Be(false);
            il.Invoke(6).Should().Be(true);
            il.Invoke(7).Should().Be(false);
            il.Invoke(8).Should().Be(true);
            il.Invoke(9).Should().Be(false);
            il.Invoke(10).Should().Be(false);
        }

        [Test]
        public void Parsing17()
        {
            var il = IL.NewMethod()
                .WithParameter(typeof(int), "a")
                .Returns(typeof(int))
                .If("a<5")
                    .Ldc(5)
                .Else()
                    .Ldarg("a")
                .EndIf()
                .Ret();

            il.Invoke(3).Should().Be(5);
            il.Invoke(4).Should().Be(5);
            il.Invoke(5).Should().Be(5);
            il.Invoke(6).Should().Be(6);
            il.Invoke(7).Should().Be(7);
            
        }

    }
}
