using System.Linq;
using System.Reflection.Emit;

// ReSharper disable CheckNamespace

namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Ldarg(params uint[] args)
        {
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case 0:
                        Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        Emit(OpCodes.Ldarg_S, (int) arg);
                        break;
                }
            }
            return this;
        }

        public DynamicMethodBody Ldarg(params string[] args)
        {
            var parameters = _methodInfo.Parameters.ToArray();
            var offset = (uint) (_methodInfo.DynamicTypeInfo != null ? 1 : 0);

            foreach (var arg in args)
                for (uint i = 0; i < parameters.Length; i++)
                    if (parameters[i].Name == arg)
                        Ldarg(i + offset);

            return this;
        }
    }
}