namespace FluentIL.ExpressionParser
{
    internal struct Token
    {
        public Token(string id, string value)
            : this()
        {
            Id = id;
            Value = value;
        }

        public string Id { get; }
        public string Value { get; }

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
            return this == (Token) obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value} ({Id})";
        }
    }
}