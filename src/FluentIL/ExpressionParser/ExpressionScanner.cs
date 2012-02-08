namespace FluentIL.ExpressionParser
{
    internal class ExpressionScanner : Scanner
    {
        public ExpressionScanner() :
            base(BuildTable())
        {
        }

        private static StateTable BuildTable()
        {
            const string whitespaces = " \t\n";
            const string digits = "0123456789";
            const string letters = "qwertyuiopasdfghjklzxcvbnm";

            return new StateTable("s1")
                .WithState("s1", new State()
                                     .WithGoTo(whitespaces, "s17")
                                     .WithGoTo(digits, "s14")
                                     .WithGoTo(letters, "s16")
                                     .WithGoTo('/', "s2")
                                     .WithGoTo('.', "s13")
                )
                .WithState("s2", new State("divide")
                                     .WithGoTo('/', "s3")
                                     .WithGoTo('*', "s4")
                )
                .WithState("s3", new State()
                                     .WithGoTo(" \t", "s3")
                                     .WithGoTo('\n', "s18")
                                     .WithGoTo("/*()+-:.", "s3")
                                     .WithGoTo(digits, "s3")
                                     .WithGoTo(letters, "s3")
                )
                .WithState("s4", new State()
                                     .WithGoTo(" \t\n/()+-:.", "s4")
                                     .WithGoTo('*', "s5")
                                     .WithGoTo(digits, "s4")
                                     .WithGoTo(letters, "s4")
                )
                .WithState("s5", new State()
                                     .WithGoTo(whitespaces, "s4")
                                     .WithGoTo('/', "s18")
                                     .WithGoTo('*', "s5")
                                     .WithGoTo("()+-:.", "s4")
                                     .WithGoTo(digits, "s4")
                                     .WithGoTo(letters, "s4")
                )
                .WithState("s12", new State("assign"))
                .WithState("s13", new State()
                                      .WithGoTo(digits, "s15")
                )
                .WithState("s14", new State("integer")
                                      .WithGoTo(digits, "s14")
                                      .WithGoTo('.', "s15")
                )
                .WithState("s15", new State("float")
                                      .WithGoTo(digits, "s15")
                )
                .WithState("s16", new State("identifier")
                                      .WithGoTo(digits, "s16")
                                      .WithGoTo(letters, "s16")
                )
                .WithState("s17", new State("white_space")
                                      .WithGoTo(whitespaces, "s17")
                )
                .WithState("s18", new State("comment"))
                .WithToken('(', "lparen")
                .WithToken(')', "rparen")
                .WithToken('+', "plus")
                .WithToken('-', "minus")
                .WithToken('*', "times")
                .WithToken('>', "gt")
                .WithToken(">=", "geq")
                .WithToken('<', "lt")
                .WithToken("<=", "leq")
                .WithToken("==", "eq")
                .WithToken("!", "not")
                .WithToken("!=", "neq")
                .WithToken("<>", "neq")
                .WithToken(":=", "assign")
                .WithToken("||", "or")
                .WithToken("&&", "and")
                .WithToken("^", "pow")
                .WithToken("%", "mod")
                ;
        }
    }
}