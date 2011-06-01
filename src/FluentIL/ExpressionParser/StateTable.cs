using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FluentIL.ExpressionParser
{
    class StateTable
    {
        public Dictionary<string, State> States { get; private set; }
        
        readonly string InitialStateId;
        public State InitialState
        { get { return States[this.InitialStateId]; } }

        public StateTable(string initialStateId)
        {
            States = new Dictionary<string, State>();
            this.InitialStateId = initialStateId;
        }

        public StateTable WithState(string key, State value)
        {
            this.States.Add(key, value);
            return this;
        }

        public StateTable WithToken(char token, string tokenId)
        {
            if (this.InitialState == null)
                throw new InvalidOperationException("Need to provide the initial state first");

            string g = Guid.NewGuid().ToString();

            this.InitialState.WithGoTo(token, g);
            return this.WithState(g, new State(tokenId));
        }

        public StateTable WithToken(string token, string tokenId)
        {
            if (this.InitialState == null)
                throw new InvalidOperationException("Need to provide the initial state first");

            var workState = this.InitialState;
            foreach (var c in token)
            {
                if (!workState.GoTo.ContainsKey(c))
                {
                    var g = Guid.NewGuid().ToString();
                    var newState = new State();
                    this.WithState(g, newState);
                    workState.WithGoTo(c, g);
                    workState = newState;
                }
                else
                {
                    workState = this.States[workState.GoTo[c]];
                }
            }
            workState.ResultingTokenId = tokenId;

            return this;
        }

        public State Accept(string stateid)
        {
            return this.States[stateid];
        }
    }
}
