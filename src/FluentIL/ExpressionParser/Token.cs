using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentIL.ExpressionParser
{
    struct Token
    {
        public string Id { get; private set; }
        public string Value { get; private set; }

        public Token(string id, string value)
            : this()
        {
            Id = id;
            Value = value;
        }

        public static bool operator ==(Token first, Token second)
        {
            return (first.Value == second.Value &&
                first.Id == second.Id);
        }

        public static bool operator !=(Token first, Token second)
        {
            return (first.Value == second.Value &&
                first.Id == second.Id);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Token)) return false;
            return this == (Token)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})",
                this.Value,
                this.Id);
        }
    }
}
