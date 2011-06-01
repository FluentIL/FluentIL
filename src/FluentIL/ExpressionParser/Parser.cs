using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FluentIL.ExpressionParser
{
    internal class Parser : IDisposable
    {
        public IEnumerable<Token> Source { get; private set; }
        public DynamicMethodBody MethodBody { get; private set; }

        internal Parser(IEnumerable<Token> source,
            DynamicMethodBody body = null)
        {
            this.Source = source;
            this.MethodBody = body;
        }

        IEnumerator<Token> SourceEnumerator;
        Token input_token;
        Token GetNextToken()
        {
            while (this.SourceEnumerator.MoveNext())
            {
                var result = this.SourceEnumerator.Current;
                if (result.Id != "white_space" && 
                    result.Id != "comment" )
                    return result;
            }

            return new Token("EOP", "EOP");
        }

        public void Dispose()
        {
            this.SourceEnumerator.Dispose();
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
            if (this.SourceEnumerator != null)
                this.SourceEnumerator.Dispose();

            this.SourceEnumerator = this.Source.GetEnumerator();
            input_token = GetNextToken();
            Begin("Expression");
            LogicalExpression();
            Match("EOP");
            End();
        }

        void LogicalExpression()
        {
            Begin("Logical");
            switch (input_token.Id)
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }

        #region OR
        
        void BooleanExpression()
        {
            Begin("Boolean");
            switch (input_token.Id)
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }
        

        void BooleanExpressionTail()
        {
            Begin("BooleanTail");
            switch (input_token.Id)
            {
                case "or":
                    OrOp();
                    var btrue = Guid.NewGuid().ToString();
                    var bend = Guid.NewGuid().ToString();
                    if (this.MethodBody != null)
                        this.MethodBody
                            .Brtrue(btrue);
                    BooleanExpression();
                    BooleanExpressionTail();
                    if (this.MethodBody != null)
                        this.MethodBody
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }

        private void OrOp()
        {
            switch (input_token.Id)
            {
                case "or":
                    Match(input_token.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(input_token);
            }
        }

        #endregion

        private void BooleanAndExpression()
        {
            Begin("BooleanAnd");
            switch (input_token.Id)
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }
        

        void BooleanAndExpressionTail()
        {
            Begin("BooleanAndTail");
            switch (input_token.Id)
            {
                case "and":
                    AndOp();
                    var bfalse = Guid.NewGuid().ToString();
                    var bend = Guid.NewGuid().ToString();
                    if (this.MethodBody != null)
                        this.MethodBody
                            .Brfalse(bfalse);
                    BooleanAndExpression();
                    BooleanAndExpressionTail();
                    if (this.MethodBody != null)
                        this.MethodBody
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }

        private void AndOp()
        {
            switch (input_token.Id)
            {
                case "and":
                    Match(input_token.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(input_token);
            }
        }


        private void EqualityExpression()
        {
            Begin("Equality");
            switch (input_token.Id)
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }


        void EqualityExpressionTail()
        {
            Begin("EqualityTail");
            switch (input_token.Id)
            {
                case "eq":
                case "neq":
                    var op = input_token.Id;
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }

        private void EqualOp()
        {
            switch (input_token.Id)
            {
                case "eq":
                case "neq":
                    Match(input_token.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(input_token);
            }
        }


        private void RelationalExpression()
        {
            Begin("Relational");
            switch (input_token.Id)
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }


        void RelationalExpressionTail()
        {
            Begin("RelationalTail");
            switch (input_token.Id)
            {
                case "lt":
                case "leq":
                case "gt":
                case "geq":
                    string op = input_token.Id;
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }

        private void RelationalOp()
        {
            switch (input_token.Id)
            {
                case "lt":
                case "leq":
                case "gt":
                case "geq":
                    Match(input_token.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(input_token);
            }
        }


        private void AdditiveExpression()
        {
            Begin("Additive");
            switch (input_token.Id)
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }


        void AdditiveExpressionTail()
        {
            Begin("AdditiveTail");
            switch (input_token.Id)
            {
                case "plus":
                case "minus":
                    string op = input_token.Id;
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }

        private void EmitOp(string operation)
        {
            if (this.MethodBody == null)
                return;

            switch (operation)
            {
                case "plus":
                    this.MethodBody.Add();
                    break;
                case "minus":
                    this.MethodBody.Sub();
                    break;
                case "times":
                    this.MethodBody.Mul();
                    break;
                case "divide":
                    this.MethodBody.Div();
                    break;
                case "mod":
                    this.MethodBody.Rem();
                    break;
                case "lt":
                    this.MethodBody.Clt();
                    break;
                case "leq":
                    this.MethodBody.Cle();
                    break;
                case "gt":
                    this.MethodBody.Cgt();
                    break;
                case "geq":
                    this.MethodBody.Cge();
                    break;
                case "eq":
                    this.MethodBody.Ceq();
                    break;
                case "neq":
                    this.MethodBody
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
            switch (input_token.Id)
            {
                case "plus":
                case "minus":
                    Match(input_token.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(input_token);
            }
        }

        private void MultiplicativeExpression()
        {
            Begin("Multiplicative");
            switch (input_token.Id)
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }


        void MultiplicativeExpressionTail()
        {
            Begin("MultiplicativeTail");
            switch (input_token.Id)
            {
                case "times":
                case "divide":
                case "mod":
                    var op = input_token.Id;
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }

        private void MultiplicativeOp()
        {

            switch (input_token.Id)
            {
                case "times":
                case "divide":
                case "mod":
                    Match(input_token.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(input_token);
            }
        }

        private void PowerExpression()
        {
            Begin("Power");
            switch (input_token.Id)
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }


        void PowerExpressionTail()
        {
            Begin("PowerTail");
            switch (input_token.Id)
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
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }

        private void PowerOp()
        {
            switch (input_token.Id)
            {
                case "pow":
                    Match(input_token.Id);
                    break;
                default:
                    throw new UnexpectedTokenException(input_token);
            }
        }

        void UnaryExpression()
        {
            Begin("Unary");
            switch (input_token.Id)
            {
                case "not":
                    Match("not");
                    PrimaryExpression();
                    if (this.MethodBody != null)
                        this.MethodBody.Ldc(0).Ceq();
                    break;
                case "minus":
                    Match("minus");
                    PrimaryExpression();
                    if (this.MethodBody != null)
                        this.MethodBody.Mul(-1);
                    break;
                case "float":
                case "integer":
                case "identifier":
                case "lparen":
                    PrimaryExpression();
                    break;
                default:
                    throw new UnexpectedTokenException(input_token);
            }
            End();
        }

        void PrimaryExpression()
        {
            Begin("Primary");
            switch (input_token.Id)
            {
                case "lparen":
                    Match("lparen");
                    LogicalExpression();
                    Match("rparen");
                    break;
                case "float":
                    if (this.MethodBody != null)
                        this.MethodBody.Ldc(float.Parse(input_token.Value));
                    Match("float");
                    break;
                case "integer":
                    if (this.MethodBody != null)
                        this.MethodBody.Ldc(int.Parse(input_token.Value));
                    Match("integer");
                    break;
                case "identifier":
                    if (this.MethodBody != null)
                        this.MethodBody.LdArgOrLoc(input_token.Value);
                    Match("identifier");
                    break;
                default:
                    throw new UnexpectedTokenException(input_token);
            }
            End();

        }


        void Skip()
        { Begin("Epsilon"); End(); }

        void Match(string tokenType)
        {
            if (input_token.Id == tokenType)
            {
                Begin(input_token.ToString());
                input_token = GetNextToken();
                End();
            }
            else
                throw new UnexpectedTokenException(input_token);
        }

        private void End()
        {
            //ident--;
        }

        int ident = 0;
        private void Begin(string p)
        {
            //Console.WriteLine("{0}{1}", new String(' ', ident), p);
            //ident++;
        }
    }

    public class UnexpectedTokenException : Exception
    {
        internal UnexpectedTokenException(Token unexpected)
            : base(string.Format("Unexpected token {0}", unexpected))
        {
        }
    }
}
