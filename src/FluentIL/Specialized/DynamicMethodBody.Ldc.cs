using System.Reflection.Emit;

// ReSharper disable CheckNamespace
namespace FluentIL
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Ldc(params string[] args)
        {
            return Ldstr(args);
        }

        public DynamicMethodBody Ldstr(params string[] args)
        {
            foreach (string arg in args)
            {
                Emit(OpCodes.Ldstr, arg);
            }
            return this;
        }

        public DynamicMethodBody Ldc(params double[] args)
        {
            return LdcR8(args);
        }

        public DynamicMethodBody LdcR8(params double[] args)
        {
            foreach (double t in args)
                Emit(OpCodes.Ldc_R8, t);

            return this;
        }

        public DynamicMethodBody Ldc(params float[] args)
        {
            return LdcR4(args);
        }

        public DynamicMethodBody LdcR4(params float[] args)
        {
            foreach (float t in args)
                Emit(OpCodes.Ldc_R4, t);

            return this;
        }

        public DynamicMethodBody Ldc(params int[] args)
        {
            return LdcI4(args);
        }

        public DynamicMethodBody LdLocOrArg(string name)
        {
            if (GetVariableIndex(name) > -1)
                return Ldloc(name);

            if (GetParameterIndex(name) > -1)
                return Ldarg(name);

            return Ldarg(0)
                .Ldfld(name);
        }

        public DynamicMethodBody LdArgOrLoc(string name)
        {
            return LdLocOrArg(name);
        }

        public DynamicMethodBody LdcI4(params int[] args)
        {
            foreach (int arg in args)
            {
                switch (arg)
                {
                    case 0:
                        Emit(OpCodes.Ldc_I4_0);
                        break;
                    case 1:
                        Emit(OpCodes.Ldc_I4_1);
                        break;
                    case 2:
                        Emit(OpCodes.Ldc_I4_2);
                        break;
                    case 3:
                        Emit(OpCodes.Ldc_I4_3);
                        break;
                    case 4:
                        Emit(OpCodes.Ldc_I4_4);
                        break;
                    case 5:
                        Emit(OpCodes.Ldc_I4_5);
                        break;
                    case 6:
                        Emit(OpCodes.Ldc_I4_6);
                        break;
                    case 7:
                        Emit(OpCodes.Ldc_I4_7);
                        break;
                    case 8:
                        Emit(OpCodes.Ldc_I4_8);
                        break;
                    case -1:
                        Emit(OpCodes.Ldc_I4_M1);
                        break;
                    default:
                        Emit(OpCodes.Ldc_I4, arg);
                        break;
                }
            }
            return this;
        }
    }
}