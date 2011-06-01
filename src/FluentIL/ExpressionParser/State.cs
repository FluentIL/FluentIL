using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentIL.ExpressionParser
{
    class State
    {
        string _ResultingTokenId = null;
        public string ResultingTokenId { 
            get
            { 
                return _ResultingTokenId; 
            }
            set
            {
                if (_ResultingTokenId != null)
                    throw new InvalidOperationException();
                _ResultingTokenId = value;
            }
        }
        public GoToDictionary GoTo { get; private set; }
        
        public bool IsStop
        { get { return ResultingTokenId != null; } }

        public State(string resultingTokenId = null)
        {
            this.ResultingTokenId = resultingTokenId;
            this.GoTo = new GoToDictionary();
        }

        public State WithGoTo(char c, string state)
        {
            this.GoTo.Add(c, state);
            return this;
        }

        public State WithGoTo(IEnumerable<char> characters, string state)
        {
            foreach (var c in characters)
                this.GoTo.Add(c, state);
            
            return this;
        }

        public bool Accepts(char symbol)
        {
            return this.GoTo.ContainsKey(symbol);
        }

        public string ChangeTo(char symbol)
        {
            return this.GoTo[symbol];
        }
    }

    public class GoToDictionary : Dictionary<char, string>
    {

    }
}
