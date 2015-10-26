using System;
using System.Collections.Generic;
using FluentIL.Emitters;

namespace FluentIL.ExpressionParser
{
    internal class Parser : IDisposable
    {
        private IEnumerator<Token> _sourceEnumerator;
        private Token _inputToken;

        internal Parser(IEnumerable<Token> source,
                        DynamicMethodBody body = null)
        {
            Source = source;
            MethodBody = body;
        }

        public IEnumerable<Token> Source { get; }
        public DynamicMethodBody MethodBody { get; }

        #region IDisposable Members

        public void Dispose()
        {
            _sourceEnumerator.Dispose();
        }

        #endregion

        private Token GetNextToken()
        {
            while (_sourceEnumerator.MoveNext())
            {
                var result = _sourceEnumerator.Current;
                if (result.Id != "white_space" &&
                    result.Id != "comment")
                    return result;
            }

            return new Token("EOP", "EOP");
        }


        public static void Parse(string expression,
                                 DynamicMethodBody methodBody = null)
        {
            new Parser(
                new ExpressionScanner().Scan(expression),
                methodBody
                ).Parse();
        }

        public void Parse()
        {
            _sourceEnumerator?.Dispose();

            _sourceEnumerator = Source.GetEnumerator();
            _inputToken = GetNextToken();
            Begin("Expression");
            LogicalExpression();
            Match("EOP");
            End();
        }

        private void LogicalExpression()
        {
            Begin("Logical");
            switch (_inputToken.Id)
            {
                case "integer":
                case "float":
                case "lparen":
                case "not":
                case "minus":
                case "identifier":
                    BooleanExpression();
                    BooleanExpressionTail();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }

        private void BooleanAndExpression()
        {
            Begin("BooleanAnd");
            switch (_inputToken.Id)
            {
                case "integer":
                case "float":
                case "not":
                case "lparen":
                case "minus":
                case "identifier":
                    EqualityExpression();
                    EqualityExpressionTail();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }


        private void BooleanAndExpressionTail()
        {
            Begin("BooleanAndTail");
            switch (_inputToken.Id)
            {
                case "and":
                    AndOp();
                    var bfalse = Guid.NewGuid().ToString();
                    var bend = Guid.NewGuid().ToString();
                    MethodBody?.Brfalse(bfalse);
                    BooleanAndExpression();
                    BooleanAndExpressionTail();
                    MethodBody?.Br_S(bend)
                        .MarkLabel(bfalse)
                        .Ldc(0)
                        .MarkLabel(bend);
                    break;
                case "rparen":
                case "or":
                case "EOP":
                    Skip();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }

        private void AndOp()
        {
            switch (_inputToken.Id)
            {
                case "and":
                    Match(_inputToken.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
        }


        private void EqualityExpression()
        {
            Begin("Equality");
            switch (_inputToken.Id)
            {
                case "integer":
                case "float":
                case "not":
                case "lparen":
                case "minus":
                case "identifier":
                    RelationalExpression();
                    RelationalExpressionTail();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }


        private void EqualityExpressionTail()
        {
            Begin("EqualityTail");
            switch (_inputToken.Id)
            {
                case "eq":
                case "neq":
                    var op = _inputToken.Id;
                    EqualOp();
                    EqualityExpression();
                    EqualityExpressionTail();
                    EmitOp(op);
                    break;
                case "rparen":
                case "or":
                case "and":
                case "EOP":
                    Skip();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }

        private void EqualOp()
        {
            switch (_inputToken.Id)
            {
                case "eq":
                case "neq":
                    Match(_inputToken.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
        }


        private void RelationalExpression()
        {
            Begin("Relational");
            switch (_inputToken.Id)
            {
                case "integer":
                case "not":
                case "float":
                case "lparen":
                case "minus":
                case "identifier":
                    AdditiveExpression();
                    AdditiveExpressionTail();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }


        private void RelationalExpressionTail()
        {
            Begin("RelationalTail");
            switch (_inputToken.Id)
            {
                case "lt":
                case "leq":
                case "gt":
                case "geq":
                    var op = _inputToken.Id;
                    RelationalOp();
                    RelationalExpression();
                    RelationalExpressionTail();
                    EmitOp(op);
                    break;
                case "rparen":
                case "eq":
                case "neq":
                case "or":
                case "and":
                case "EOP":
                    Skip();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }

        private void RelationalOp()
        {
            switch (_inputToken.Id)
            {
                case "lt":
                case "leq":
                case "gt":
                case "geq":
                    Match(_inputToken.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
        }


        private void AdditiveExpression()
        {
            Begin("Additive");
            switch (_inputToken.Id)
            {
                case "integer":
                case "float":
                case "lparen":
                case "not":
                case "minus":
                case "identifier":
                    MultiplicativeExpression();
                    MultiplicativeExpressionTail();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }


        private void AdditiveExpressionTail()
        {
            Begin("AdditiveTail");
            switch (_inputToken.Id)
            {
                case "plus":
                case "minus":
                    var op = _inputToken.Id;
                    AdditiveOp();
                    AdditiveExpression();
                    AdditiveExpressionTail();
                    EmitOp(op);
                    break;
                case "rparen":
                case "EOP":
                case "lt":
                case "leq":
                case "gt":
                case "eq":
                case "or":
                case "neq":
                case "and":
                case "geq":
                    Skip();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }

        private void EmitOp(string operation)
        {
            if (MethodBody == null)
                return;

            switch (operation)
            {
                case "plus":
                    MethodBody.Add();
                    break;
                case "minus":
                    MethodBody.Sub();
                    break;
                case "times":
                    MethodBody.Mul();
                    break;
                case "divide":
                    MethodBody.Div();
                    break;
                case "mod":
                    MethodBody.Rem();
                    break;
                case "lt":
                    MethodBody.Clt();
                    break;
                case "leq":
                    MethodBody.Cle();
                    break;
                case "gt":
                    MethodBody.Cgt();
                    break;
                case "geq":
                    MethodBody.Cge();
                    break;
                case "eq":
                    MethodBody.Ceq();
                    break;
                case "neq":
                    MethodBody
                        .Ceq()
                        .Ldc(0)
                        .Ceq();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void AdditiveOp()
        {
            switch (_inputToken.Id)
            {
                case "plus":
                case "minus":
                    Match(_inputToken.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
        }

        private void MultiplicativeExpression()
        {
            Begin("Multiplicative");
            switch (_inputToken.Id)
            {
                case "integer":
                case "float":
                case "lparen":
                case "minus":
                case "not":
                case "identifier":
                    PowerExpression();
                    PowerExpressionTail();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }


        private void MultiplicativeExpressionTail()
        {
            Begin("MultiplicativeTail");
            switch (_inputToken.Id)
            {
                case "times":
                case "divide":
                case "mod":
                    var op = _inputToken.Id;
                    MultiplicativeOp();
                    MultiplicativeExpression();
                    MultiplicativeExpressionTail();
                    EmitOp(op);
                    break;
                case "rparen":
                case "plus":
                case "minus":
                case "lt":
                case "leq":
                case "gt":
                case "geq":
                case "eq":
                case "or":
                case "and":
                case "neq":
                case "EOP":
                    Skip();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }

        private void MultiplicativeOp()
        {
            switch (_inputToken.Id)
            {
                case "times":
                case "divide":
                case "mod":
                    Match(_inputToken.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
        }

        private void PowerExpression()
        {
            Begin("Power");
            switch (_inputToken.Id)
            {
                case "integer":
                case "float":
                case "lparen":
                case "not":
                case "minus":
                case "identifier":
                    UnaryExpression();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }


        private void PowerExpressionTail()
        {
            Begin("PowerTail");
            switch (_inputToken.Id)
            {
                case "pow":
                    PowerOp();
                    PowerExpression();
                    PowerExpressionTail();
                    break;
                case "rparen":
                case "plus":
                case "minus":
                case "times":
                case "divide":
                case "mod":
                case "lt":
                case "leq":
                case "gt":
                case "geq":
                case "eq":
                case "neq":
                case "or":
                case "and":
                case "EOP":
                    Skip();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }

        private void PowerOp()
        {
            switch (_inputToken.Id)
            {
                case "pow":
                    Match(_inputToken.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
        }

        private void UnaryExpression()
        {
            Begin("Unary");
            switch (_inputToken.Id)
            {
                case "not":
                    Match("not");
                    PrimaryExpression();
                    MethodBody?.Ldc(0).Ceq();
                    break;
                case "minus":
                    Match("minus");
                    PrimaryExpression();
                    MethodBody?.Mul(-1);
                    break;
                case "float":
                case "integer":
                case "identifier":
                case "lparen":
                    PrimaryExpression();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }

        private void PrimaryExpression()
        {
            Begin("Primary");
            switch (_inputToken.Id)
            {
                case "lparen":
                    Match("lparen");
                    LogicalExpression();
                    Match("rparen");
                    break;
                case "float":
                    MethodBody?.Ldc(float.Parse(_inputToken.Value));
                    Match("float");
                    break;
                case "integer":
                    MethodBody?.Ldc(int.Parse(_inputToken.Value));
                    Match("integer");
                    break;
                case "identifier":
                    MethodBody?.LdArgOrLoc(_inputToken.Value);
                    Match("identifier");
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }


        private void Skip()
        {
            Begin("Epsilon");
            End();
        }

        private void Match(string tokenType)
        {
            if (_inputToken.Id == tokenType)
            {
                Begin(_inputToken.ToString());
                _inputToken = GetNextToken();
                End();
            }
            else
                throw new UnexpectedTokenException(_inputToken);
        }

        private static void End()
        {
            //ident--;
        }

// ReSharper disable UnusedParameter.Local
        private static void Begin(string p)
// ReSharper restore UnusedParameter.Local
        {
            //Console.WriteLine("{0}{1}", new String(' ', ident), p);
            //ident++;
        }

        #region OR

        private void BooleanExpression()
        {
            Begin("Boolean");
            switch (_inputToken.Id)
            {
                case "integer":
                case "float":
                case "lparen":
                case "not":
                case "minus":
                case "identifier":
                    BooleanAndExpression();
                    BooleanAndExpressionTail();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }


        private void BooleanExpressionTail()
        {
            Begin("BooleanTail");
            switch (_inputToken.Id)
            {
                case "or":
                    OrOp();
                    var btrue = Guid.NewGuid().ToString();
                    var bend = Guid.NewGuid().ToString();
                    MethodBody?.Brtrue(btrue);
                    BooleanExpression();
                    BooleanExpressionTail();
                    MethodBody?.Br_S(bend)
                        .MarkLabel(btrue)
                        .Ldc(1)
                        .MarkLabel(bend);
                    break;
                case "rparen":
                case "EOP":
                    Skip();
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
            End();
        }

        private void OrOp()
        {
            switch (_inputToken.Id)
            {
                case "or":
                    Match(_inputToken.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(_inputToken);
            }
        }

        #endregion
    }

    public class UnexpectedTokenException : Exception
    {
        internal UnexpectedTokenException(Token unexpected)
            : base($"Unexpected token {unexpected}")
        {
        }
    }
}