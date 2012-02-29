using System.Collections.Generic;
using System.Reflection.Emit;
using FluentIL.Numbers;

// ReSharper disable CheckNamespace
namespace FluentIL
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        private readonly Stack<ForInfo> forsField = new Stack<ForInfo>();

        public DynamicMethodBody Emit(params Number[] numbers)
        {
            foreach (Number number in numbers)
                number.Emit(this);
            return this;
        }


        public DynamicMethodBody For(string variable, Number from, Number to, int step = 1)
        {
            ILGenerator ilgen = methodInfoField.GetILGenerator();
            Label beginLabel = ilgen.DefineLabel();
            Label comparasionLabel = ilgen.DefineLabel();

            forsField.Push(new ForInfo(variable, from, to, step,
                                       beginLabel, comparasionLabel));
            if (GetVariableIndex(variable) == -1)
            {
                methodInfoField.WithVariable(typeof (int), variable);
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
    }
}