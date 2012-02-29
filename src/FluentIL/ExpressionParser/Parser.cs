using System;
using System.Collections.Generic;
using FluentIL.Emitters;

namespace FluentIL.ExpressionParser
{
    internal class Parser : IDisposable
    {
        private IEnumerator<Token> sourceEnumeratorField;
        private Token inputTokenField;

        internal Parser(IEnumerable<Token> source,
                        DynamicMethodBody body = null)
        {
            Source = source;
            MethodBody = body;
        }

        public IEnumerable<Token> Source { get; private set; }
        public DynamicMethodBody MethodBody { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            sourceEnumeratorField.Dispose();
        }

        #endregion

        private Token GetNextToken()
        {
            while (sourceEnumeratorField.MoveNext())
            {
                Token result = sourceEnumeratorField.Current;
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
            if (sourceEnumeratorField != null)
                sourceEnumeratorField.Dispose();

            sourceEnumeratorField = Source.GetEnumerator();
            inputTokenField = GetNextToken();
            Begin("Expression");
            LogicalExpression();
            Match("EOP");
            End();
        }

        private void LogicalExpression()
        {
            Begin("Logical");
            switch (inputTokenField.Id)
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }

        private void BooleanAndExpression()
        {
            Begin("BooleanAnd");
            switch (inputTokenField.Id)
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }


        private void BooleanAndExpressionTail()
        {
            Begin("BooleanAndTail");
            switch (inputTokenField.Id)
            {
                case "and":
                    AndOp();
                    string bfalse = Guid.NewGuid().ToString();
                    string bend = Guid.NewGuid().ToString();
                    if (MethodBody != null)
                        MethodBody
                            .Brfalse(bfalse);
                    BooleanAndExpression();
                    BooleanAndExpressionTail();
                    if (MethodBody != null)
                        MethodBody
                            .Br_S(bend)
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }

        private void AndOp()
        {
            switch (inputTokenField.Id)
            {
                case "and":
                    Match(inputTokenField.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(inputTokenField);
            }
        }


        private void EqualityExpression()
        {
            Begin("Equality");
            switch (inputTokenField.Id)
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }


        private void EqualityExpressionTail()
        {
            Begin("EqualityTail");
            switch (inputTokenField.Id)
            {
                case "eq":
                case "neq":
                    string op = inputTokenField.Id;
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }

        private void EqualOp()
        {
            switch (inputTokenField.Id)
            {
                case "eq":
                case "neq":
                    Match(inputTokenField.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(inputTokenField);
            }
        }


        private void RelationalExpression()
        {
            Begin("Relational");
            switch (inputTokenField.Id)
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }


        private void RelationalExpressionTail()
        {
            Begin("RelationalTail");
            switch (inputTokenField.Id)
            {
                case "lt":
                case "leq":
                case "gt":
                case "geq":
                    string op = inputTokenField.Id;
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }

        private void RelationalOp()
        {
            switch (inputTokenField.Id)
            {
                case "lt":
                case "leq":
                case "gt":
                case "geq":
                    Match(inputTokenField.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(inputTokenField);
            }
        }


        private void AdditiveExpression()
        {
            Begin("Additive");
            switch (inputTokenField.Id)
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }


        private void AdditiveExpressionTail()
        {
            Begin("AdditiveTail");
            switch (inputTokenField.Id)
            {
                case "plus":
                case "minus":
                    string op = inputTokenField.Id;
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
                    throw new UnexpectedTokenException(inputTokenField);
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
            switch (inputTokenField.Id)
            {
                case "plus":
                case "minus":
                    Match(inputTokenField.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(inputTokenField);
            }
        }

        private void MultiplicativeExpression()
        {
            Begin("Multiplicative");
            switch (inputTokenField.Id)
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }


        private void MultiplicativeExpressionTail()
        {
            Begin("MultiplicativeTail");
            switch (inputTokenField.Id)
            {
                case "times":
                case "divide":
                case "mod":
                    string op = inputTokenField.Id;
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }

        private void MultiplicativeOp()
        {
            switch (inputTokenField.Id)
            {
                case "times":
                case "divide":
                case "mod":
                    Match(inputTokenField.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(inputTokenField);
            }
        }

        private void PowerExpression()
        {
            Begin("Power");
            switch (inputTokenField.Id)
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }


        private void PowerExpressionTail()
        {
            Begin("PowerTail");
            switch (inputTokenField.Id)
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }

        private void PowerOp()
        {
            switch (inputTokenField.Id)
            {
                case "pow":
                    Match(inputTokenField.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(inputTokenField);
            }
        }

        private void UnaryExpression()
        {
            Begin("Unary");
            switch (inputTokenField.Id)
            {
                case "not":
                    Match("not");
                    PrimaryExpression();
                    if (MethodBody != null)
                        MethodBody.Ldc(0).Ceq();
                    break;
                case "minus":
                    Match("minus");
                    PrimaryExpression();
                    if (MethodBody != null)
                        MethodBody.Mul(-1);
                    break;
                case "float":
                case "integer":
                case "identifier":
                case "lparen":
                    PrimaryExpression();
                    break;
                default:
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }

        private void PrimaryExpression()
        {
            Begin("Primary");
            switch (inputTokenField.Id)
            {
                case "lparen":
                    Match("lparen");
                    LogicalExpression();
                    Match("rparen");
                    break;
                case "float":
                    if (MethodBody != null)
                        MethodBody.Ldc(float.Parse(inputTokenField.Value));
                    Match("float");
                    break;
                case "integer":
                    if (MethodBody != null)
                        MethodBody.Ldc(int.Parse(inputTokenField.Value));
                    Match("integer");
                    break;
                case "identifier":
                    if (MethodBody != null)
                        MethodBody.LdArgOrLoc(inputTokenField.Value);
                    Match("identifier");
                    break;
                default:
                    throw new UnexpectedTokenException(inputTokenField);
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
            if (inputTokenField.Id == tokenType)
            {
                Begin(inputTokenField.ToString());
                inputTokenField = GetNextToken();
                End();
            }
            else
                throw new UnexpectedTokenException(inputTokenField);
        }

        private void End()
        {
            //ident--;
        }

// ReSharper disable UnusedParameter.Local
        private void Begin(string p)
// ReSharper restore UnusedParameter.Local
        {
            //Console.WriteLine("{0}{1}", new String(' ', ident), p);
            //ident++;
        }

        #region OR

        private void BooleanExpression()
        {
            Begin("Boolean");
            switch (inputTokenField.Id)
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
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }


        private void BooleanExpressionTail()
        {
            Begin("BooleanTail");
            switch (inputTokenField.Id)
            {
                case "or":
                    OrOp();
                    string btrue = Guid.NewGuid().ToString();
                    string bend = Guid.NewGuid().ToString();
                    if (MethodBody != null)
                        MethodBody
                            .Brtrue(btrue);
                    BooleanExpression();
                    BooleanExpressionTail();
                    if (MethodBody != null)
                        MethodBody
                            .Br_S(bend)
                            .MarkLabel(btrue)
                            .Ldc(1)
                            .MarkLabel(bend);
                    break;
                case "rparen":
                case "EOP":
                    Skip();
                    break;
                default:
                    throw new UnexpectedTokenException(inputTokenField);
            }
            End();
        }

        private void OrOp()
        {
            switch (inputTokenField.Id)
            {
                case "or":
                    Match(inputTokenField.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(inputTokenField);
            }
        }

        #endregion
    }

    public class UnexpectedTokenException : Exception
    {
        internal UnexpectedTokenException(Token unexpected)
            : base(string.Format("Unexpected token {0}", unexpected))
        {
        }
    }
}