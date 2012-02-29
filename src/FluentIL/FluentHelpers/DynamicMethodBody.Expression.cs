using System.Linq.Expressions;
using FluentIL.ExpressionInterpreter;
using FluentIL.ExpressionParser;

namespace FluentIL
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
