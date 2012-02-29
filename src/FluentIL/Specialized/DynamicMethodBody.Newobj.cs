using System;
using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable CheckNamespace
namespace FluentIL
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
            ConstructorInfo ci = typeof (T).GetConstructor(types);
            return Newobj(ci);
        }
    }
}