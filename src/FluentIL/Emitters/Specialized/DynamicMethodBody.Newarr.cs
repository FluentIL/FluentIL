using System;
using System.Reflection.Emit;
using FluentIL.Numbers;

// ReSharper disable CheckNamespace

namespace FluentIL.Emitters
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