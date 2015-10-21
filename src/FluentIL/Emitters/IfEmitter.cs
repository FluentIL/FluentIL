using System;
using System.Reflection.Emit;

namespace FluentIL.Emitters
{
    internal class IfEmitter
    {
        public readonly string LeftSideVarName;
        private readonly string _doneField;
        private readonly DynamicMethodBody _generatorField;
        private readonly string _ifFalseField;
        private bool _withElseField;

        public IfEmitter(DynamicMethodBody generator)
        {
            _generatorField = generator;
            _ifFalseField = $"IfFalse_{Guid.NewGuid()}";
            _doneField = $"Done_{Guid.NewGuid()}";
            LeftSideVarName = $"LS_{Guid.NewGuid()}";
        }

        public bool MultipleConditions { get; set; }

        public void EmitIf(OpCode comparasionOpcode, bool not = false)
        {
            _generatorField
                .Emit(comparasionOpcode)
                .EmitIf(!not, a => a.Brfalse(_ifFalseField))
                .EmitIf(not, a => a.Brtrue(_ifFalseField));
        }


        public void EmitBranch(bool not = false)
        {
            _generatorField
                .EmitIf(!not, a => a.Brfalse(_ifFalseField))
                .EmitIf(not, a => a.Brtrue(_ifFalseField));
        }

        public void EmitElse()
        {
            _generatorField
                .Br(_doneField)
                .MarkLabel(_ifFalseField);

            _withElseField = true;
        }

        public void EmitEndIf()
        {
            if (!_withElseField) _generatorField.MarkLabel(_ifFalseField);
            _generatorField.MarkLabel(_doneField);
        }
    }
}