using System;
using System.Reflection.Emit;

namespace FluentIL.Emitters
{
    internal class IfEmitter
    {
        public readonly string LeftSideVarName;
        private readonly string _done;
        private readonly DynamicMethodBody _generator;
        private readonly string _ifFalse;
        private bool _withElse;

        public IfEmitter(DynamicMethodBody generator)
        {
            _generator = generator;
            _ifFalse = $"IfFalse_{Guid.NewGuid()}";
            _done = $"Done_{Guid.NewGuid()}";
            LeftSideVarName = $"LS_{Guid.NewGuid()}";
        }

        public bool MultipleConditions { get; set; }

        public void EmitIf(OpCode comparasionOpcode, bool not = false)
        {
            _generator
                .Emit(comparasionOpcode)
                .EmitIf(!not, a => a.Brfalse(_ifFalse))
                .EmitIf(not, a => a.Brtrue(_ifFalse));
        }


        public void EmitBranch(bool not = false)
        {
            _generator
                .EmitIf(!not, a => a.Brfalse(_ifFalse))
                .EmitIf(not, a => a.Brtrue(_ifFalse));
        }

        public void EmitElse()
        {
            _generator
                .Br(_done)
                .MarkLabel(_ifFalse);

            _withElse = true;
        }

        public void EmitEndIf()
        {
            if (!_withElse) _generator.MarkLabel(_ifFalse);
            _generator.MarkLabel(_done);
        }
    }
}