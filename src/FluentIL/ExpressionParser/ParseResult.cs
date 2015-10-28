using System;

namespace FluentIL.ExpressionParser
{
    public class ParseResult
    {
        public Type ExpressionType { get; internal set; }

        internal void AnalyzeType(Type type)
        {
            var shouldUse =
                (ExpressionType == null) ||
                (type == typeof(bool)) ||
                (type == typeof(double) && ExpressionType == typeof(int));

            if (shouldUse)
            {
                ExpressionType = type;
            }
        }
    }
}