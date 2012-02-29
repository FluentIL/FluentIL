using FluentIL.ExpressionParser;

namespace FluentIL.Numbers
{
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
}