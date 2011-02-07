using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentIL
{
    public struct DynamicVariableInfo
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        public DynamicVariableInfo(string name, Type type)
            : this()
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
