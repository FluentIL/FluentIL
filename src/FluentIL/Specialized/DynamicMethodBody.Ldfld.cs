using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable CheckNamespace
namespace FluentIL
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Ldfld(FieldInfo fldInfo)
        {
            Emit(OpCodes.Ldfld, fldInfo);
            return this;
        }

        public DynamicMethodBody Ldfld(string fieldName)
        {
            FieldInfo field = methodInfoField.DynamicTypeInfo.GetFieldInfo(fieldName);
            return Ldfld(field);
        }


        public DynamicMethodBody Ldsfld(FieldInfo fieldInfo)
        {
            return Emit(OpCodes.Ldsfld, fieldInfo);
        }
    }
}