using System;
using System.Collections.Generic;

namespace FluentIL.ExpressionParser
{
    internal class StateTable
    {
        private readonly string _initialStateIdField;

        public StateTable(string initialStateId)
        {
            States = new Dictionary<string, State>();
            _initialStateIdField = initialStateId;
        }

        public Dictionary<string, State> States { get; private set; }

        public State InitialState => States[_initialStateIdField];

        public StateTable WithState(string key, State value)
        {
            States.Add(key, value);
            return this;
        }

        public StateTable WithToken(char token, string tokenId)
        {
            if (InitialState == null)
                throw new InvalidOperationException("Need to provide the initial state first");

            var g = Guid.NewGuid().ToString();

            InitialState.WithGoTo(token, g);
            return WithState(g, new State(tokenId));
        }

        public StateTable WithToken(string token, string tokenId)
        {
            if (InitialState == null)
                throw new InvalidOperationException("Need to provide the initial state first");

            var workState = InitialState;
            foreach (var c in token)
            {
                if (!workState.GoTo.ContainsKey(c))
                {
                    var g = Guid.NewGuid().ToString();
                    var newState = new State();
                    WithState(g, newState);
                    workState.WithGoTo(c, g);
                    workState = newState;
                }
                else
                {
                    workState = States[workState.GoTo[c]];
                }
            }
            workState.ResultingTokenId = tokenId;

            return this;
        }

        public State Accept(string stateid)
        {
            return States[stateid];
        }
    }
}