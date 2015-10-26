using System;
using System.Collections.Generic;
using FluentIL.Infos;
using FluentIL.Numbers;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        private readonly Stack<ForInfo> _fors = new Stack<ForInfo>();

        public DynamicMethodBody Emit(params Number[] numbers)
        {
            foreach (var number in numbers)
                number.Emit(this);
            return this;
        }


        public DynamicMethodBody For(string variable, Number from, Number to, int step = 1)
        {
            var ilgen = _methodInfo.GetILEmitter();
            var beginLabel = ilgen.DefineLabel();
            var comparasionLabel = ilgen.DefineLabel();

            _fors.Push(new ForInfo(variable, from, to, step,
                                       beginLabel, comparasionLabel));
            if (GetVariableIndex(variable) == -1)
            {
                _methodInfo.WithVariable(typeof (int), variable);
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
            var f = _fors.Pop();
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

        public DynamicMethodBody For(
            string variable, 
            Number from, 
            Number to,
            Action<DynamicMethodBody> @do
            )
        {
            For(variable, from, to);
                @do(this);
            Next();

            return this;
        }

        public DynamicMethodBody For(
            string variable,
            Number from,
            Number to,
            int step,
            Action<DynamicMethodBody> @do
            )
        {
            For(variable, from, to, step);
                @do(this);
            Next();

            return this;
        }
    }
}