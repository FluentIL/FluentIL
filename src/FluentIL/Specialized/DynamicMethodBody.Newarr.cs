using System;
using System.Reflection.Emit;

// ReSharper disable CheckNamespace

namespace FluentIL
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Newarr(Type type)
        {
            return Emit(OpCodes.Newarr, type);
        }

        public DynamicMethodBody Newarr(Type type, Number size)
        {
            return Emit(size)
                .Emit(OpCodes.Newarr, type);
        }
    }
}