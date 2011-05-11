using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace FluentIL
{
    public class DynamicFieldInfo
    {
        public DynamicTypeInfo DynamicTypeInfo { get; private set; }
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public FieldBuilder FieldBuilder { get; internal set; }

        public DynamicFieldInfo(
            DynamicTypeInfo dti,
            string name,
            Type type, 
            FieldBuilder fieldBuilder = null)
        {
            this.DynamicTypeInfo = dti;
            this.Name = name;
            this.Type = type;
            this.FieldBuilder = fieldBuilder;
        }
    }
}
