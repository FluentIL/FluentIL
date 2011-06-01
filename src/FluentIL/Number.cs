using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
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
        public int Value { get; private set; }

        public ConstantInt32Number(int value)
        {
            this.Value = value;
        }

        public override void Emit(DynamicMethodBody generator)
        {
            generator.Ldc(this.Value);
        }
    }

    public class ConstantDoubleNumber : Number
    {
        public double Value { get; private set; }

        public ConstantDoubleNumber(double value)
        {
            this.Value = value;
        }

        public override void Emit(DynamicMethodBody generator)
        {
            generator.Ldc(this.Value);
        }
    }

    public class ParseExpressionNumber : Number
    {
        public string Expression { get; private set; }
        public ParseExpressionNumber(string expression)
        {
            this.Expression = expression;
        }

        public override void Emit(DynamicMethodBody generator)
        {
            Parser.Parse(this.Expression, generator);
        }
    }

    public class ExpressionNumber : Number
    {
        public Expression Expression { get; private set; }
        public ExpressionNumber(Expression expression)
        {
            this.Expression = expression;
        }

        public override void Emit(DynamicMethodBody generator)
        {
            generator.Expression(this.Expression);
        }

    }
}
