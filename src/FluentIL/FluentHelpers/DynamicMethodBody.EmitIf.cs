using System;

// ReSharper disable CheckNamespace
namespace FluentIL
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody EmitIf(bool condition, Action<DynamicMethodBody> action)
        {
            if (condition)
                action(this);

            return this;
        }
    }
}