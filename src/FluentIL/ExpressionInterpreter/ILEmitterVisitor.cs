using System;
using System.Linq.Expressions;
using FluentIL.Emitters;

namespace FluentIL.ExpressionInterpreter
{
    public class ILEmitterVisitor : ExpressionVisitor
    {
// ReSharper disable InconsistentNaming
        private readonly DynamicMethodBody IL;
// ReSharper restore InconsistentNaming

        public ILEmitterVisitor(DynamicMethodBody dmb)
        {
            IL = dmb;
        }


        protected override Expression VisitParameter(ParameterExpression node)
        {
            var result = base.VisitParameter(node);
            IL.LdLocOrArg(node.Name);
            return result;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Expression result;

            switch (node.NodeType)
            {
                case ExpressionType.AndAlso:
                {
                    var firstConditionIsFalse = Guid.NewGuid().ToString();
                    var jump = Guid.NewGuid().ToString();

                    result = node;
                    Visit(node.Left);
                    IL.Brfalse(firstConditionIsFalse);
                    Visit(node.Right);
                    IL
                        .Br_S(jump)
                        .MarkLabel(firstConditionIsFalse)
                        .Ldc(0)
                        .MarkLabel(jump);
                }
                    break;
                case ExpressionType.OrElse:
                {
                    var firstConditionIsTrue = Guid.NewGuid().ToString();
                    var jump = Guid.NewGuid().ToString();

                    result = node;
                    Visit(node.Left);
                    IL.Brtrue(firstConditionIsTrue);
                    Visit(node.Right);
                    IL
                        .Br_S(jump)
                        .MarkLabel(firstConditionIsTrue)
                        .Ldc(1)
                        .MarkLabel(jump);
                }
                    break;
                default:
                    result = base.VisitBinary(node);
                    switch (node.NodeType)
                    {
                        case ExpressionType.Add:
                            IL.Add();
                            break;
                        case ExpressionType.Divide:
                            IL.Div();
                            break;
                        case ExpressionType.Multiply:
                            IL.Mul();
                            break;
                        case ExpressionType.Subtract:
                            IL.Sub();
                            break;
                        case ExpressionType.GreaterThan:
                            IL.Cgt();
                            break;
                        case ExpressionType.GreaterThanOrEqual:
                            IL.Cge();
                            break;
                        case ExpressionType.LessThan:
                            IL.Clt();
                            break;
                        case ExpressionType.LessThanOrEqual:
                            IL.Cle();
                            break;
                        case ExpressionType.Equal:
                            IL.Ceq();
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    break;
            }

            return result;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var result = base.VisitConstant(node);

            if (node.Type == typeof (int))
                IL.Ldc((int) node.Value);
            else if (node.Type == typeof (double))
                IL.Ldc((double) node.Value);
            else if (node.Type == typeof (float))
                IL.Ldc((float) node.Value);
            else if (node.Type == typeof (bool))
                IL.Ldc((bool) node.Value ? 1 : 0);
            else
                throw new NotSupportedException();

            return result;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var result = base.VisitUnary(node);
            if (node.NodeType == ExpressionType.Not)
                IL.Ldc(0).Ceq();
            return result;
        }
    }
}