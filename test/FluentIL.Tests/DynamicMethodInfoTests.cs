using System;
using System.Linq;
using System.Reflection.Emit;
using FluentIL.Infos;
using NUnit.Framework;
using SharpTestsEx;

namespace FluentIL.Tests
{
    [TestFixture]
    public class DynamicMethodInfoTests
    {
        [Test]
        public void ctor_implicitConvertionToDyamicMethod_ReturnTypeShouldBeVoid()
        {
            DynamicMethod dm = new DynamicMethodInfo();

            dm.ReturnType.Should().Be(typeof(void));
        }

        [Test]
        public void WithParameter_OneTimePassingInt_DynamicMethodShouldHaveOneParamInt()
        {
            // arrange
            DynamicMethod dm = new DynamicMethodInfo()
                .WithParameter(typeof(int));

            // act

            // assert  
            dm.GetParameters().First().ParameterType.Should().Be(typeof(int));

        }

        [Test]
        public void WithParameter_TwoTimesPassingInt_DynamicMethodShouldHaveOneParamInt()
        {
            // arrange
            DynamicMethod dm = new DynamicMethodInfo()
                .WithParameter(typeof(int))
                .WithParameter(typeof(int));

            // act

            // assert  
            dm.GetParameters().Select(p => p.ParameterType)
                .Should().Have.SameSequenceAs(
                    typeof(int),
                    typeof(int)
                );

        }

        [Test]
        public void WithParameter_PassingIntAnsString_DynamicMethodShouldHaveOneParamIntAndAnotherString()
        {
            // arrange
            DynamicMethod dm = new DynamicMethodInfo()
                .WithParameter(typeof(int))
                .WithParameter(typeof(string));

            // act

            // assert  
            dm.GetParameters().Select(p => p.ParameterType)
                .Should().Have.SameSequenceAs(
                    typeof(int),
                    typeof(string)
                );

        }

        [Test]
        public void ctor_implicitConvertionToDyamicMethod_ShouldHaveNoParameters()
        {
            DynamicMethod dm = new DynamicMethodInfo();

            dm.GetParameters().Count().Should().Be(0);
        }

        [Test]
        public void Returns_PassingInt_ReturnTypeShouldBeInt()
        {
            DynamicMethod dm = new DynamicMethodInfo()
                .Returns(typeof(int));

            dm.ReturnType.Should().Be(typeof(int));
        }

        [Test]
        public void Returns_PassingAny_ShouldReturnSameValueOfBodyProperty()
        {
            // arrange 
            var dmi = new DynamicMethodInfo();
            var expected = dmi.Body;

            // act
            var result = dmi.Returns(typeof(int));

            // assert
            result.Should().Be(expected);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AsDynamicMethod_WithNotNullDynamicTypeInfo_ThrowsInvalidOperationException()
        {
            // arrange
            var dti = new DynamicTypeInfo("Type");
            var dmi = new DynamicMethodInfo(dti, "Name");
            // act
            var result = dmi.AsDynamicMethod;
            // assert
        }
    }
}
