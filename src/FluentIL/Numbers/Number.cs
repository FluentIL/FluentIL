using System.Linq.Expressions;
using FluentIL.Emitters;

namespace FluentIL.Numbers
{
    public abstract class Number
    {
        public abstract void Emit(DynamicMethodBody generator);

        public static implicit operator Number(int value)
        {
            return new ConstantInt32Number(value);
        }

        public static implicit operator Number(double value)
        {
            return new ConstantDoubleNumber(value);
        }

        public static implicit operator Number(string varName)
        {
            return new ParseExpressionNumber(varName);
        }

        public static implicit operator Number(Expression expression)
        {
            return new ExpressionNumber(expression);
        }
    }
}