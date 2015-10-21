using System;
using System.Collections.Generic;
using System.Text;

namespace FluentIL.ExpressionParser
{
    internal class Scanner
    {
        public Scanner(StateTable statesTable)
        {
            if (statesTable == null) throw new ArgumentNullException(nameof(statesTable));
            StatesTable = statesTable;
        }

        public StateTable StatesTable { get; private set; }


        public IEnumerable<Token> Scan(IEnumerable<char> source)
        {
            State[] current = {StatesTable.InitialState};
            var position = 0;
            var @continue = false;
            var currentValue = new StringBuilder();
            var symbol = ' ';

            Func<Token> markPartialStop = () =>
            {
                if (current[0].IsStop)
                {
                    var result = new Token(current[0].ResultingTokenId,
                                            currentValue.ToString());
                    current[0] = StatesTable.InitialState;
                    currentValue.Clear();
                    return result;
                }
                else
                    throw new ArgumentException(
                        $"Invalid source (construction {currentValue}; symbol '{(@continue ? symbol.ToString() : "EOP")}' pos {position})",
                        nameof(source));
            };

            using (var enumerator = source.GetEnumerator())
            {
                @continue = enumerator.MoveNext();
                while (@continue)
                {
                    symbol = @enumerator.Current;
                    if (current[0].Accepts(symbol))
                    {
                        current[0] = StatesTable.Accept(current[0].ChangeTo(symbol));
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