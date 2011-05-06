using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace FluentIL.ExpressionInterpreter
{
    public class ExpressionSimplifierVisitor :
        ExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Add)
            {
                var left = Visit(node.Left);
                var right = Visit(node.Right);

                if (left is ConstantExpression && right is ConstantExpression)
                {
                    if (node.Type == typeof(int))
                    {
                        int lvalue = (int)((ConstantExpression)left).Value;
                        int rvalue = (int)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue + rvalue);
                    }
                    else if (node.Type == typeof(double))
                    {
                        double lvalue = (double)((ConstantExpression)left).Value;
                        double rvalue = (double)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue + rvalue);
                    }
                    else if (node.Type == typeof(float))
                    {
                        double lvalue = (float)((ConstantExpression)left).Value;
                        double rvalue = (float)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue + rvalue);
                    }
                }
            }
            return base.VisitBinary(node);
        }
    }
}
