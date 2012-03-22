using System;
using System.Reflection.Emit;

namespace FluentIL.Infos
{
    public class DynamicFieldInfo
    {
        public DynamicFieldInfo(
            DynamicTypeInfo dti,
            string name,
            Type type,
            FieldBuilder fieldBuilder = null)
        {
            DynamicTypeInfo = dti;
            Name = name;
            Type = type;
            FieldBuilder = fieldBuilder;
        }

        public DynamicTypeInfo DynamicTypeInfo { get; private set; }
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public FieldBuilder FieldBuilder { get; internal set; }
    }
}