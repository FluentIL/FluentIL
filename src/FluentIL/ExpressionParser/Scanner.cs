using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FluentIL.ExpressionParser
{
    class Scanner
    {
        public StateTable StatesTable { get; private set; }

        public Scanner(StateTable statesTable)
        {
            if (statesTable == null) throw new ArgumentNullException("states");
            this.StatesTable = statesTable;
        }


        public IEnumerable<Token> Scan(IEnumerable<char> source) 
        { 
            var currentState = StatesTable.InitialState;
            int position = 0;
            bool @continue = false;
            StringBuilder currentValue = new StringBuilder();
            char symbol = ' ';
            Func<Token> markPartialStop = () => { 
                if (currentState.IsStop) 
                { 
                    var result = new Token(currentState.ResultingTokenId, 
                        currentValue.ToString()); 
                    currentState = StatesTable.InitialState; 
                    currentValue.Clear(); 
                    return result; 
                } 
                else            
                    throw new ArgumentException(
                        string.Format("Invalid source (construction {0}; symbol '{1}' pos {2})",
                            currentValue.ToString(), (@continue ? symbol.ToString() : "EOP"), position ),
                        "source"); 
            };
            using (var enumerator = source.GetEnumerator())
            {
                @continue = enumerator.MoveNext();
                while (@continue)
                {
                    symbol = @enumerator.Current;
                    if (currentState.Accepts(symbol))
                    {
                        currentState = StatesTable.Accept(currentState.ChangeTo(symbol));
                        currentValue.Append(symbol);
                        @continue = enumerator.MoveNext();
                        position++;
                    }
                    else
                        yield return markPartialStop();
                }
            }
            yield return markPartialStop(); 
        }
    }
}
