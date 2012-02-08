using System;
using System.Collections.Generic;

namespace FluentIL.ExpressionParser
{
    internal class State
    {
        private string resultingTokenIdField;

        public State(string resultingTokenId = null)
        {
            ResultingTokenId = resultingTokenId;
            GoTo = new GoToDictionary();
        }

        public string ResultingTokenId
        {
            get { return resultingTokenIdField; }
            set
            {
                if (resultingTokenIdField != null)
                    throw new InvalidOperationException();
                resultingTokenIdField = value;
            }
        }

        public GoToDictionary GoTo { get; private set; }

        public bool IsStop
        {
            get { return ResultingTokenId != null; }
        }

        public State WithGoTo(char c, string state)
        {
            GoTo.Add(c, state);
            return this;
        }

        public State WithGoTo(IEnumerable<char> characters, string state)
        {
            foreach (char c in characters)
                GoTo.Add(c, state);

            return this;
        }

        public bool Accepts(char symbol)
        {
            return GoTo.ContainsKey(symbol);
        }

        public string ChangeTo(char symbol)
        {
            return GoTo[symbol];
        }
    }

    public class GoToDictionary : Dictionary<char, string>
    {
    }
}