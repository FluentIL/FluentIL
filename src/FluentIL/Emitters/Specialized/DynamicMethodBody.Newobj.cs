using System;
using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Newobj(ConstructorInfo ctorInfo)
        {
            return Emit(OpCodes.Newobj, ctorInfo);
        }

        public DynamicMethodBody Newobj<T>(params Type[] types)
        {
            var ci = typeof (T).GetConstructor(types);
            return Newobj(ci);
        }
    }
}