using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable CheckNamespace

namespace FluentIL.Emitters
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
            var field = _methodInfoField.DynamicTypeInfo.GetFieldInfo(fieldName);
            return Stfld(field);
        }
    }
}