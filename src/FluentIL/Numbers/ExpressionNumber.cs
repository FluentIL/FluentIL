using System.Linq.Expressions;
using FluentIL.Emitters;

namespace FluentIL.Numbers
{
    public class ExpressionNumber : Number
    {
        public ExpressionNumber(Expression expression)
        {
            Expression = expression;
        }

        public Expression Expression { get; private set; }

        public override void Emit(DynamicMethodBody generator)
        {
            generator.Expression(Expression);
        }
    }
}