using System.Linq.Expressions;
using FluentIL.ExpressionInterpreter;
using FluentIL.ExpressionParser;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Parse(string expression)
        {
            Parser.Parse(expression, this);
            return this;
        }

        public DynamicMethodBody Expression(Expression expression)
        {
            expression = new ExpressionSimplifierVisitor().Visit(expression);
            new ILEmitterVisitor(this).Visit(
                expression
                );
            return this;
        }
    }
}
