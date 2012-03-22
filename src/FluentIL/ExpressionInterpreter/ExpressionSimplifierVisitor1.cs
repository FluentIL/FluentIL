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
                    if (left.Type == typeof(int) && right.Type == typeof(int))
                    {
                        var lvalue = (int)((ConstantExpression)left).Value;
                        var rvalue = (int)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue + rvalue);
                    }
                    
					if (left.Type == typeof(double) && right.Type == typeof(double))
                    {
                        var lvalue = (double)((ConstantExpression)left).Value;
                        var rvalue = (double)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue + rvalue);
                    }
                    
					if (left.Type == typeof(float)  && right.Type == typeof(float))
                    {
                        var lvalue = (float)((ConstantExpression)left).Value;
                        var rvalue = (float)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue + rvalue);
                    }
                }
            }

            if (node.NodeType == ExpressionType.Subtract)
            {
                var left = Visit(node.Left);
                var right = Visit(node.Right);

                if (left is ConstantExpression && right is ConstantExpression)
                {
                    if (left.Type == typeof(int) && right.Type == typeof(int))
                    {
                        var lvalue = (int)((ConstantExpression)left).Value;
                        var rvalue = (int)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue - rvalue);
                    }
                    
					if (left.Type == typeof(double) && right.Type == typeof(double))
                    {
                        var lvalue = (double)((ConstantExpression)left).Value;
                        var rvalue = (double)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue - rvalue);
                    }
                    
					if (left.Type == typeof(float)  && right.Type == typeof(float))
                    {
                        var lvalue = (float)((ConstantExpression)left).Value;
                        var rvalue = (float)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue - rvalue);
                    }
                }
            }

            if (node.NodeType == ExpressionType.Multiply)
            {
                var left = Visit(node.Left);
                var right = Visit(node.Right);

                if (left is ConstantExpression && right is ConstantExpression)
                {
                    if (left.Type == typeof(int) && right.Type == typeof(int))
                    {
                        var lvalue = (int)((ConstantExpression)left).Value;
                        var rvalue = (int)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue * rvalue);
                    }
                    
					if (left.Type == typeof(double) && right.Type == typeof(double))
                    {
                        var lvalue = (double)((ConstantExpression)left).Value;
                        var rvalue = (double)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue * rvalue);
                    }
                    
					if (left.Type == typeof(float)  && right.Type == typeof(float))
                    {
                        var lvalue = (float)((ConstantExpression)left).Value;
                        var rvalue = (float)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue * rvalue);
                    }
                }
            }

            if (node.NodeType == ExpressionType.Divide)
            {
                var left = Visit(node.Left);
                var right = Visit(node.Right);

                if (left is ConstantExpression && right is ConstantExpression)
                {
                    if (left.Type == typeof(int) && right.Type == typeof(int))
                    {
                        var lvalue = (int)((ConstantExpression)left).Value;
                        var rvalue = (int)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue / rvalue);
                    }
                    
					if (left.Type == typeof(double) && right.Type == typeof(double))
                    {
                        var lvalue = (double)((ConstantExpression)left).Value;
                        var rvalue = (double)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue / rvalue);
                    }
                    
					if (left.Type == typeof(float)  && right.Type == typeof(float))
                    {
                        var lvalue = (float)((ConstantExpression)left).Value;
                        var rvalue = (float)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue / rvalue);
                    }
                }
            }

            if (node.NodeType == ExpressionType.GreaterThanOrEqual)
            {
                var left = Visit(node.Left);
                var right = Visit(node.Right);

                if (left is ConstantExpression && right is ConstantExpression)
                {
                    if (left.Type == typeof(int) && right.Type == typeof(int))
                    {
                        var lvalue = (int)((ConstantExpression)left).Value;
                        var rvalue = (int)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue >= rvalue);
                    }
                    
					if (left.Type == typeof(double) && right.Type == typeof(double))
                    {
                        var lvalue = (double)((ConstantExpression)left).Value;
                        var rvalue = (double)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue >= rvalue);
                    }
                    
					if (left.Type == typeof(float)  && right.Type == typeof(float))
                    {
                        var lvalue = (float)((ConstantExpression)left).Value;
                        var rvalue = (float)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue >= rvalue);
                    }
                }
            }

            if (node.NodeType == ExpressionType.LessThanOrEqual)
            {
                var left = Visit(node.Left);
                var right = Visit(node.Right);

                if (left is ConstantExpression && right is ConstantExpression)
                {
                    if (left.Type == typeof(int) && right.Type == typeof(int))
                    {
                        var lvalue = (int)((ConstantExpression)left).Value;
                        var rvalue = (int)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue <= rvalue);
                    }
                    
					if (left.Type == typeof(double) && right.Type == typeof(double))
                    {
                        var lvalue = (double)((ConstantExpression)left).Value;
                        var rvalue = (double)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue <= rvalue);
                    }
                    
					if (left.Type == typeof(float)  && right.Type == typeof(float))
                    {
                        var lvalue = (float)((ConstantExpression)left).Value;
                        var rvalue = (float)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue <= rvalue);
                    }
                }
            }

            if (node.NodeType == ExpressionType.GreaterThan)
            {
                var left = Visit(node.Left);
                var right = Visit(node.Right);

                if (left is ConstantExpression && right is ConstantExpression)
                {
                    if (left.Type == typeof(int) && right.Type == typeof(int))
                    {
                        var lvalue = (int)((ConstantExpression)left).Value;
                        var rvalue = (int)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue > rvalue);
                    }
                    
					if (left.Type == typeof(double) && right.Type == typeof(double))
                    {
                        var lvalue = (double)((ConstantExpression)left).Value;
                        var rvalue = (double)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue > rvalue);
                    }
                    
					if (left.Type == typeof(float)  && right.Type == typeof(float))
                    {
                        var lvalue = (float)((ConstantExpression)left).Value;
                        var rvalue = (float)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue > rvalue);
                    }
                }
            }

            if (node.NodeType == ExpressionType.LessThan)
            {
                var left = Visit(node.Left);
                var right = Visit(node.Right);

                if (left is ConstantExpression && right is ConstantExpression)
                {
                    if (left.Type == typeof(int) && right.Type == typeof(int))
                    {
                        var lvalue = (int)((ConstantExpression)left).Value;
                        var rvalue = (int)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue < rvalue);
                    }
                    
					if (left.Type == typeof(double) && right.Type == typeof(double))
                    {
                        var lvalue = (double)((ConstantExpression)left).Value;
                        var rvalue = (double)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue < rvalue);
                    }
                    
					if (left.Type == typeof(float)  && right.Type == typeof(float))
                    {
                        var lvalue = (float)((ConstantExpression)left).Value;
                        var rvalue = (float)((ConstantExpression)right).Value;
                        return Expression.Constant(lvalue < rvalue);
                    }
                }
            }

            return base.VisitBinary(node);
        }
    }
}
