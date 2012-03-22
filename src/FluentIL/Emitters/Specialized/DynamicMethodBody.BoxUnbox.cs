using System;
using System.Reflection.Emit;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Box(Type type)
        {
            if (type.IsSubclassOf(typeof(ValueType)))
                Emit(OpCodes.Box, type);
            return this;
        }

        public DynamicMethodBody UnboxAny(Type type)
        {
            return Emit(OpCodes.Unbox_Any, type);
        }
    }
}
