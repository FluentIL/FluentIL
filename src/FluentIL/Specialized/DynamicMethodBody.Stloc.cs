// ReSharper disable CheckNamespace

using FluentIL.Numbers;

namespace FluentIL
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Stloc(Number value, params string[] variables)
        {
            Emit(value);

            for (int i = 1; i < variables.Length; i++)
                Dup();

            Stloc(variables);

            return this;
        }
    }
}