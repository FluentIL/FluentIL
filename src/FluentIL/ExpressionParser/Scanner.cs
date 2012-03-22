using System;
using System.Collections.Generic;
using System.Text;

namespace FluentIL.ExpressionParser
{
    internal class Scanner
    {
        public Scanner(StateTable statesTable)
        {
            if (statesTable == null) throw new ArgumentNullException("statesTable");
            StatesTable = statesTable;
        }

        public StateTable StatesTable { get; private set; }


        public IEnumerable<Token> Scan(IEnumerable<char> source)
        {
            State current = StatesTable.InitialState;
            int position = 0;
            bool @continue = false;
            var currentValue = new StringBuilder();
            char symbol = ' ';
            Func<Token> markPartialStop = () =>
                                              {
                                                  if (current.IsStop)
                                                  {
                                                      var result = new Token(current.ResultingTokenId,
                                                                             currentValue.ToString());
                                                      current = StatesTable.InitialState;
                                                      currentValue.Clear();
                                                      return result;
                                                  }
                                                  else
                                                      throw new ArgumentException(
                                                          string.Format(
                                                              "Invalid source (construction {0}; symbol '{1}' pos {2})",
                                                              currentValue, (@continue ? symbol.ToString() : "EOP"),
                                                              position),
                                                          "source");
                                              };
            using (IEnumerator<char> enumerator = source.GetEnumerator())
            {
                @continue = enumerator.MoveNext();
                while (@continue)
                {
                    symbol = @enumerator.Current;
                    if (current.Accepts(symbol))
                    {
                        current = StatesTable.Accept(current.ChangeTo(symbol));
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