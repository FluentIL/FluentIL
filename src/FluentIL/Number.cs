using System.Linq.Expressions;
using FluentIL.ExpressionParser;

namespace FluentIL
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

    public class ConstantInt32Number : Number
    {
        public ConstantInt32Number(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }

        public override void Emit(DynamicMethodBody generator)
        {
            generator.Ldc(Value);
        }
    }

    public class ConstantDoubleNumber : Number
    {
        public ConstantDoubleNumber(double value)
        {
            Value = value;
        }

        public double Value { get; private set; }

        public override void Emit(DynamicMethodBody generator)
        {
            generator.Ldc(Value);
        }
    }

    public class ParseExpressionNumber : Number
    {
        public ParseExpressionNumber(string expression)
        {
            Expression = expression;
        }

        public string Expression { get; private set; }

        public override void Emit(DynamicMethodBody generator)
        {
            Parser.Parse(Expression, generator);
        }
    }

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