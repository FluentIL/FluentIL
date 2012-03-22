using System;
using System.Reflection.Emit;

namespace FluentIL.Emitters
{
    internal class IfEmitter
    {
        public readonly string LeftSideVarName;
        private readonly string doneField;
        private readonly DynamicMethodBody generatorField;
        private readonly string ifFalseField;
        private bool withElseField;

        public IfEmitter(DynamicMethodBody generator)
        {
            generatorField = generator;
            ifFalseField = string.Format("IfFalse_{0}", Guid.NewGuid());
            doneField = string.Format("Done_{0}", Guid.NewGuid());
            LeftSideVarName = string.Format("LS_{0}", Guid.NewGuid());
        }

        public bool MultipleConditions { get; set; }

        public void EmitIf(OpCode comparasionOpcode, bool not = false)
        {
            generatorField
                .Emit(comparasionOpcode)
                .EmitIf(!not, a => a.Brfalse(ifFalseField))
                .EmitIf(not, a => a.Brtrue(ifFalseField));
        }


        public void EmitBranch(bool not = false)
        {
            generatorField
                .EmitIf(!not, a => a.Brfalse(ifFalseField))
                .EmitIf(not, a => a.Brtrue(ifFalseField));
        }

        public void EmitElse()
        {
            generatorField
                .Br(doneField)
                .MarkLabel(ifFalseField);

            withElseField = true;
        }

        public void EmitEndIf()
        {
            if (!withElseField) generatorField.MarkLabel(ifFalseField);
            generatorField.MarkLabel(doneField);
        }
    }
}