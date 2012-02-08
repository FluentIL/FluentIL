using System;

namespace FluentIL
{
    public struct DynamicVariableInfo
    {
        public DynamicVariableInfo(string name, Type type)
            : this()
        {
            Name = name;
            Type = type;
        }

        public string Name { get; private set; }
        public Type Type { get; private set; }
    }
}