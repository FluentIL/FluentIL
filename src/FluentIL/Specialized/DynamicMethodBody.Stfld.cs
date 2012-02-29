using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable CheckNamespace

namespace FluentIL
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Stfld(FieldInfo fldInfo)
        {
            Emit(OpCodes.Stfld, fldInfo);
            return this;
        }

        public DynamicMethodBody Stfld(string fieldName)
        {
            FieldInfo field = methodInfoField.DynamicTypeInfo.GetFieldInfo(fieldName);
            return Stfld(field);
        }
    }
}