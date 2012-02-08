using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using FluentIL.ExpressionInterpreter;
using FluentIL.ExpressionParser;

namespace FluentIL
{
    public partial class DynamicMethodBody
    {
        private readonly DynamicMethodInfo infoField;

        internal DynamicMethodBody(DynamicMethodInfo info)
        {
            infoField = info;
        }

        public DynamicMethod AsDynamicMethod
        {
            get { return infoField.AsDynamicMethod; }
        }

        public Type AsType
        {
            get { return infoField.DynamicTypeInfo.AsType; }
        }


        public DynamicMethodInfo WithMethod(string methodName)
        {
            return infoField.DynamicTypeInfo.WithMethod(methodName);
        }

        public DynamicMethodBody Parse(string expression)
        {
            Parser.Parse(expression, this);
            return this;
        }


        public DynamicMethodBody Box(Type type)
        {
            if (type.IsSubclassOf(typeof (ValueType)))
                Emit(OpCodes.Box, type);
            return this;
        }

        public DynamicMethodBody UnboxAny(Type type)
        {
            return Emit(OpCodes.Unbox_Any, type);
        }

        public DynamicMethodBody Ldfld(FieldInfo fldInfo)
        {
            Emit(OpCodes.Ldfld, fldInfo);
            return this;
        }

        public DynamicMethodBody Ldfld(string fieldName)
        {
            FieldInfo field = infoField.DynamicTypeInfo.GetFieldInfo(fieldName);
            return Ldfld(field);
        }

        public DynamicMethodBody Stfld(FieldInfo fldInfo)
        {
            Emit(OpCodes.Stfld, fldInfo);
            return this;
        }

        public DynamicMethodBody Stfld(string fieldName)
        {
            FieldInfo field = infoField.DynamicTypeInfo.GetFieldInfo(fieldName);
            return Stfld(field);
        }


        public DynamicMethodBody Throw<TException>(params Type[] types)
            where TException : Exception
        {
            return Newobj<TException>(types)
                .Throw();
        }

        public DynamicMethodBody Ldsfld(FieldInfo fieldInfo)
        {
            return Emit(OpCodes.Ldsfld, fieldInfo);
        }

        public DynamicMethodBody Newobj(ConstructorInfo ctorInfo)
        {
            return Emit(OpCodes.Newobj, ctorInfo);
        }

        public DynamicMethodBody Newarr(Type type)
        {
            return Emit(OpCodes.Newarr, type);
        }

        public DynamicMethodBody Newarr(Type type, Number size)
        {
            return Emit(size)
                .Emit(OpCodes.Newarr, type);
        }

        public DynamicMethodBody Newobj<T>(params Type[] types)
        {
            ConstructorInfo ci = typeof (T).GetConstructor(types);
            return Newobj(ci);
        }

        public DynamicMethodBody Expression(Expression expression)
        {
            expression = new ExpressionSimplifierVisitor().Visit(expression);
            new ILEmitterVisitor(this).Visit(
                expression
                );
            return this;
        }

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
            return infoField.AsDynamicMethod.Invoke(null, args);
        }

        #region extended Stloc

        public DynamicMethodBody Stloc(Number value, params string[] variables)
        {
            Emit(value);

            for (int i = 1; i < variables.Length; i++)
                Dup();

            Stloc(variables);

            return this;
        }

        #endregion

        #region AddToVar

        public DynamicMethodBody AddToVar(string varname, Number constant)
        {
            return Ldloc(varname)
                .Add(constant)
                .Stloc(varname);
        }

        public DynamicMethodBody AddToVar(string varname)
        {
            return Ldloc(varname)
                .Add()
                .Stloc(varname);
        }

        #endregion

        #region EnsureLimits

        public DynamicMethodBody EnsureLimits(Number min, Number max)
        {
            return Dup()
                .Emit(min)
                .Iflt()
                .Pop()
                .Emit(min)
                .Else()
                .Dup()
                .Emit(max)
                .Ifgt()
                .Pop()
                .Emit(max)
                .EndIf()
                .EndIf();
        }

        #endregion

        #region static

        public static implicit operator DynamicMethod(DynamicMethodBody body)
        {
            return body.infoField;
        }

        public static implicit operator DynamicMethodInfo(DynamicMethodBody body)
        {
            return body.infoField;
        }

        #endregion

        #region Basic Math Operations

        private void MultipleOperations(Func<DynamicMethodBody> action, params Number[] args)
        {
            Emit(args);
            if (args.Length == 1)
                action();
            else
                for (int i = 0; i < args.Length - 1; i++)
                    action();
        }


        public DynamicMethodBody Ret(bool returnValue)
        {
            return Ldc(returnValue ? 1 : 0)
                .Ret();
        }
       
// ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Rem(params Number[] args)
// ReSharper restore MethodOverloadWithOptionalParameter
        {
            if (args == null) throw new ArgumentNullException("args");
            MultipleOperations(Rem, args);
            return this;
        }


// ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Add(params Number[] args)
// ReSharper restore MethodOverloadWithOptionalParameter
        {
            if (args == null) throw new ArgumentNullException("args");
            MultipleOperations(Add, args);
            return this;
        }

        public DynamicMethodBody Add(Number arg)
        {
            return Emit(arg).Add();
        }

        private const double EPSILON = 0.001;
// ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Mul(params Number[] args)
// ReSharper restore MethodOverloadWithOptionalParameter
        {
            if (args.Length == 1 && args[0] is ConstantDoubleNumber)
            {
                var constantDoubleNumber = args[0] as ConstantDoubleNumber;
// ReSharper disable PossibleNullReferenceException
                double factor = constantDoubleNumber.Value;
// ReSharper restore PossibleNullReferenceException
                if (Math.Abs(factor - 1) < EPSILON)
                    return this;
                if (Math.Abs(factor - -1) < EPSILON)
                    return Neg();
                return
                    LdcR8(factor).Mul();
            }
            
            MultipleOperations(Mul, args);
            return this;
        }


// ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Div(params Number[] args)
// ReSharper restore MethodOverloadWithOptionalParameter
        {
            MultipleOperations(Div, args);
            return this;
        }


// ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Sub(params Number[] args)
// ReSharper restore MethodOverloadWithOptionalParameter
        {
            MultipleOperations(Sub, args);
            return this;
        }

        #endregion

        #region Locals (variables)

        public int GetVariableIndex(string varname)
        {
            DynamicVariableInfo[] variables = infoField.Variables.ToArray();

            for (int i = 0; i < variables.Length; i++)
                if (variables[i].Name == varname)
                    return i;

            return -1;
        }

        public int GetParameterIndex(string parametername)
        {
            DynamicVariableInfo[] parameters = infoField.Parameters.ToArray();

            for (int i = 0; i < parameters.Length; i++)
                if (parameters[i].Name == parametername)
                    return i;

            return -1;
        }


        public DynamicMethodBody Ldloc(params uint[] args)
        {
            foreach (uint arg in args)
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
                        Emit(OpCodes.Ldloc, (int) arg);
                        break;
                }
            }
            return this;
        }

        public DynamicMethodBody Stloc(params uint[] args)
        {
            foreach (uint arg in args)
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
                        Emit(OpCodes.Stloc, (int) arg);
                        break;
                }
            }
            return this;
        }

        public DynamicMethodBody Ldloc(params string[] args)
        {
            foreach (string arg in args)
                Ldloc((uint) GetVariableIndex(arg));

            return this;
        }

        public DynamicMethodBody Stloc(params string[] args)
        {
            foreach (string arg in args)
                Stloc((uint) GetVariableIndex(arg));

            return this;
        }

        #endregion

        #region Arguments (Parameters)

        public DynamicMethodBody Ldarg(params uint[] args)
        {
            foreach (uint arg in args)
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
            DynamicVariableInfo[] parameters = infoField.Parameters.ToArray();
            var offset = (uint) (infoField.DynamicTypeInfo != null ? 1 : 0);

            foreach (string arg in args)
                for (uint i = 0; i < parameters.Length; i++)
                    if (parameters[i].Name == arg)
                        Ldarg(i + offset);

            return this;
        }

        #endregion

        #region Constants

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

        #endregion

        #region Labels

        private readonly Dictionary<string, Label> labelsField = new Dictionary<string, Label>();

        public DynamicMethodBody MarkLabel(Label label)
        {
            Debug.Print("IL_{0}:", label.GetHashCode());

            infoField.GetILGenerator()
                .MarkLabel(label);

            return this;
        }

        public DynamicMethodBody MarkLabel(string label)
        {
            Label lbl = GetLabel(label);
            Debug.Print("IL_{0}:", lbl.GetHashCode());

            infoField.GetILGenerator()
                .MarkLabel(GetLabel(label));

            return this;
        }

        private Label GetLabel(string label)
        {
            if (!labelsField.ContainsKey(label))
                labelsField.Add(label, infoField.GetILGenerator().DefineLabel());

            return labelsField[label];
        }

        #endregion

        #region For..Next

        private readonly Stack<ForInfo> forsField = new Stack<ForInfo>();

        public DynamicMethodBody Emit(params Number[] numbers)
        {
            foreach (Number number in numbers)
                number.Emit(this);
            return this;
        }


        public DynamicMethodBody For(string variable, Number from, Number to, int step = 1)
        {
            ILGenerator ilgen = infoField.GetILGenerator();
            Label beginLabel = ilgen.DefineLabel();
            Label comparasionLabel = ilgen.DefineLabel();

            forsField.Push(new ForInfo(variable, from, to, step,
                                   beginLabel, comparasionLabel));
            if (GetVariableIndex(variable) == -1)
            {
                infoField.WithVariable(typeof (int), variable);
                ilgen.DeclareLocal(typeof (int));
            }

            Emit(from)
                .Stloc(variable)
                .Br(comparasionLabel)
                .MarkLabel(beginLabel);

            return this;
        }

        public DynamicMethodBody Next()
        {
            ForInfo f = forsField.Pop();
            Ldloc(f.Variable)
                .Ldc(f.Step)
                .Add()
                .Stloc(f.Variable)
                .MarkLabel(f.ComparasionLabel)
                .Ldloc(f.Variable)
                .Emit(f.To);

            if (f.Step > 0)
                Ble(f.BeginLabel);
            else
                Bge(f.BeginLabel);

            return this;
        }

        #endregion

        #region Abs

        public DynamicMethodBody AbsR8()
        {
            return Dup()
                .Iflt(0.0)
                .Neg()
                .EndIf();
        }

        public DynamicMethodBody AbsI4()
        {
            return Dup()
                .Iflt(0)
                .Neg()
                .EndIf();
        }

        #endregion
    }
}