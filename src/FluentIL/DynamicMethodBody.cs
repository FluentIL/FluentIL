using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace FluentIL
{
    public partial class DynamicMethodBody
    {
        DynamicMethodInfo _Info;
        internal DynamicMethodBody(DynamicMethodInfo info)
        {
            this._Info = info;
        }

        public DynamicMethod AsDynamicMethod
        {
            get
            {
                return _Info.AsDynamicMethod;
            }
        }

        public Type AsType
        {
            get
            {
                return _Info.DynamicTypeInfo.AsType;
            }
        }

        public DynamicMethodInfo WithMethod(string methodName)
        {
            return this._Info.DynamicTypeInfo.WithMethod(methodName);
        }


        #region Basic Math Operations
        private void MultipleOperations(Func<DynamicMethodBody> action, params int[] args)
        {
            this.LdcI4(args);
            if (args.Length == 1)
                action();
            else
                for (int i = 0; i < args.Length - 1; i++)
                    action();
        }

        private void MultipleOperations(Func<DynamicMethodBody> action, params double[] args)
        {
            this.LdcR8(args);
            if (args.Length == 1)
                action();
            else
                for (int i = 0; i < args.Length - 1; i++)
                    action();
        }


        public DynamicMethodBody Add()
        {
            return this.Emit(OpCodes.Add);
        }

        public DynamicMethodBody Add(params int [] args)
        {
            this.MultipleOperations(this.Add, args);
            return this;
        }

        public DynamicMethodBody Add(int arg)
        {
            if (arg == 0) return this;
            return this.LdcI4(arg).Add();
        }

        public DynamicMethodBody Add(params double[] args)
        {
            this.MultipleOperations(this.Add, args);
            return this;
        }

        public DynamicMethodBody Add(double arg)
        {
            if (arg == 0) return this;
            return this.LdcR8(arg).Add();
        }

        public DynamicMethodBody Mul()
        {
            return this.Emit(OpCodes.Mul);
        }


        public DynamicMethodBody Mul(params int[] args)
        {
            this.MultipleOperations(this.Mul, args);
            return this;
        }

        public DynamicMethodBody Mul(double factor)
        {
            if (factor == 1) 
                return this;
            if (factor == -1) 
                return this.Neg();
            return 
                this.LdcR8(factor).Mul();
        }

        public DynamicMethodBody Mul(params double[] args)
        {
            this.MultipleOperations(this.Mul, args);
            return this;
        }

        public DynamicMethodBody Div()
        {
            return this.Emit(OpCodes.Div);
        }

        public DynamicMethodBody Div(params int[] args)
        {
            this.MultipleOperations(this.Div, args);
            return this;
        }

        public DynamicMethodBody Sub()
        {
            return this.Emit(OpCodes.Sub);
        }

        public DynamicMethodBody Sub(params int[] args)
        {
            this.MultipleOperations(this.Sub, args);
            return this;
        }
        #endregion

        #region Locals (variables)

        public int GetVariableIndex(string varname)
        {
            var variables = _Info.Variables.ToArray();

            for (int i = 0; i < variables.Length; i++)
                if (variables[i].Name == varname)
                    return i;

            return -1;
        }


        public DynamicMethodBody Ldloc(params uint [] args)
        {
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case 0:
                        Emit(OpCodes.Ldloc_0);
                        break;
                    case 1:
                        Emit(OpCodes.Ldloc_1);
                        break;
                    case 2:
                        Emit(OpCodes.Ldloc_2);
                        break;
                    case 3:
                        Emit(OpCodes.Ldloc_3);
                        break;
                    default:
                        Emit(OpCodes.Ldloc, (int)arg);
                        break;

                }
            }
            return this;
        }

        public DynamicMethodBody Stloc(params uint[] args)
        {
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case 0:
                        Emit(OpCodes.Stloc_0);
                        break;
                    case 1:
                        Emit(OpCodes.Stloc_1);
                        break;
                    case 2:
                        Emit(OpCodes.Stloc_2);
                        break;
                    case 3:
                        Emit(OpCodes.Stloc_3);
                        break;
                    default:
                        Emit(OpCodes.Stloc, (int)arg);
                        break;

                }
            }
            return this;
        }

        public DynamicMethodBody Ldloc(params string[] args)
        {
            foreach (var arg in args)
                Ldloc((uint)GetVariableIndex(arg));

            return this;
        }

        public DynamicMethodBody Stloc(params string[] args)
        {
            var variables = _Info.Variables.ToArray();

            foreach (var arg in args)
                Stloc((uint)GetVariableIndex(arg));

            return this;
        }
        #endregion

        #region Arguments (Parameters)
        
        public DynamicMethodBody Ldarg(params uint [] args)
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
                        Emit(OpCodes.Ldarg_S, (int)arg);
                        break;

                }
            }
            return this;
        }

        public DynamicMethodBody Ldarg(params string[] args)
        {
            var parameters = _Info.Parameters.ToArray();
            uint offset = (uint) (_Info.DynamicTypeInfo != null ? 1 : 0);

            foreach (var arg in args)
                for (uint i = 0; i < parameters.Length; i++)
                    if (parameters[i].Name == arg)
                        Ldarg(i + offset);

            return this;
        }
        #endregion

        #region Constants
        public DynamicMethodBody Ldc(params string[] args)
        {
            return this.Ldstr(args);
        }
        
        public DynamicMethodBody Ldstr(params string[] args)
        {
            foreach (var arg in args)
            {
                Emit(OpCodes.Ldstr, arg);
            }
            return this;
        }

        public DynamicMethodBody Ldc(params double[] args)
        {
            return this.LdcR8(args);
        }
        
        public DynamicMethodBody LdcR8(params double[]  args)
        {
            for (int i = 0; i < args.Length; i++)
                Emit(OpCodes.Ldc_R8, args[i]);

            return this;
        }

        public DynamicMethodBody Ldc(params float[] args)
        {
            return this.LdcR4(args);
        }

        public DynamicMethodBody LdcR4(params float[] args)
        {
            for (int i = 0; i < args.Length; i++)
                Emit(OpCodes.Ldc_R4, args[i]);

            return this;
        }
        
        public DynamicMethodBody Ldc(params int[] args)
        {
            return this.LdcI4(args);
        }

        public DynamicMethodBody LdcI4(params int[] args)
        {
            foreach (var arg in args)
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
        #endregion

        #region Labels
        public DynamicMethodBody MarkLabel(Label label)
        {
            _Info.GetILGenerator()
                .MarkLabel(label);

            return this;
        }

        public DynamicMethodBody MarkLabel(string label)
        {
            _Info.GetILGenerator()
                .MarkLabel(GetLabel(label));

            return this;
        }
        
        readonly Dictionary<string, Label> _Labels = new Dictionary<string, Label>();
        Label GetLabel(string label)
        {
            if (!_Labels.ContainsKey(label))
                _Labels.Add(label, _Info.GetILGenerator().DefineLabel());

            return _Labels[label];
        }
        #endregion

        #region For..Next
        readonly Stack<ForInfo> _Fors = new Stack<ForInfo>();
        public DynamicMethodBody For(string variable, int from, int to, int step = 1)
        {

            var ilgen = this._Info.GetILGenerator();
            var lbl = ilgen.DefineLabel();

            _Fors.Push(new ForInfo(variable, from, to, step, lbl));
            if (GetVariableIndex(variable) == -1)
            {
                this._Info.WithVariable(typeof(int), variable);
                ilgen.DeclareLocal(typeof(int));
            }

            this
                .LdcI4(from)
                .Stloc(variable)
                .MarkLabel(lbl);
               
            return this;
        }

        public DynamicMethodBody Next()
        {
            var f = _Fors.Pop();
            this
                .Ldloc(f.Variable)
                .Add(f.Step)
                .Stloc(f.Variable)

                .Ldloc(f.Variable)
                .LdcI4(f.To);

            if (f.From < f.To)
                this.Ble(f.GoTo);
            else
                this.Bge(f.GoTo);

            return this;
        }
        #endregion

        #region Abs
        public DynamicMethodBody AbsR8()
        {
            return  this
                .Dup()
                .Iflt(0.0)
                    .Neg()
                .EndIf();
        }

        public DynamicMethodBody AbsI4()
        {
            return this
                .Dup()
                .Iflt(0)
                    .Neg()
                .EndIf();
        }
        #endregion

        #region extended Stloc
        public DynamicMethodBody Stloc(double value, params string [] variables)
        {
            this.Ldc(value);

            for (int i = 1; i < variables.Length; i++)
                this.Dup();

            this.Stloc(variables);

            return this;
        }
        #endregion

        #region AddToVar
        public DynamicMethodBody AddToVar(string varname, int constant)
        {
            return this
                .Ldloc(varname)
                .Add(constant)
                .Stloc(varname);
        }

        public DynamicMethodBody AddToVar(string varname, double constant)
        {
            return this
                .Ldloc(varname)
                .Add(constant)
                .Stloc(varname);
        }

        public DynamicMethodBody AddToVar(string varname)
        {
            return this
                .Ldloc(varname)
                .Add()
                .Stloc(varname);
        }
        #endregion

        #region EnsureLimits
        public DynamicMethodBody EnsureLimits(int min, int max)
        {
            return this
                .Dup()
                .LdcI4(min)
                .Iflt()
                    .Pop()
                    .LdcI4(min)
                .Else()
                    .Dup()
                    .LdcI4(max)
                    .Ifgt()
                        .Pop()
                        .LdcI4(max)
                    .EndIf()
                .EndIf();
        }

        public DynamicMethodBody EnsureLimits(double min, double max)
        {
            return this
                .Dup()
                .LdcR8(min)
                .Iflt()
                    .Pop()
                    .LdcR8(min)
                .Else()
                    .Dup()
                    .LdcR8(max)
                    .Ifgt()
                        .Pop()
                        .LdcR8(max)
                    .EndIf()
                .EndIf();
        }
        #endregion

        #region static
        public static implicit operator DynamicMethod(DynamicMethodBody body)
        {
            return body._Info;
        }

        public static implicit operator DynamicMethodInfo(DynamicMethodBody body)
        {
            return body._Info;
        }
        #endregion

        public DynamicMethodBody Repeater(int from, int to, int step,
            Action<int, DynamicMethodBody> action
            )
        {
            for (int i = from; i <= to; i += step)
                action(i, this);

            return this;
        }

        public DynamicMethodBody Repeater(int from, int to, int step, 
            Func<int, DynamicMethodBody, bool> precondition,
            Action<int, DynamicMethodBody> action
            )
        {
            for (int i = from; i <= to; i += step)
                if (precondition(i, this))
                    action(i, this);

            return this;
        }


        public DynamicMethodBody EmitIf(bool condition, Action<DynamicMethodBody> action)
        {
            if (condition)
                action(this);

            return this;
        }

        public object Invoke(params object[] args)
        {
            return _Info.AsDynamicMethod.Invoke(null, args);
        }
        
    }
}
