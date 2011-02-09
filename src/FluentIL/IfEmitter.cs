using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace FluentIL
{
    class IfEmitter
    {
        readonly DynamicMethodBody _Generator;
        readonly string _IfFalse;
        readonly string _Done;
        bool _WithElse;
        
        public IfEmitter(DynamicMethodBody generator)
        {
            _Generator = generator;
            _IfFalse = string.Format("IfFalse_{0}", Guid.NewGuid());
            _Done = string.Format("IfFalse_{0}", Guid.NewGuid());
        }

        public void EmitIf(OpCode comparasionOpcode)
        {
            _Generator
                .Emit(comparasionOpcode)
                .Brfalse(_IfFalse);
        }

        public void EmitElse()
        {
            _Generator
                .Br(_Done)
                .MarkLabel(_IfFalse);

            _WithElse = true;
        }

        public void EmitEndIf()
        {
            if (!_WithElse) _Generator.MarkLabel(_IfFalse);
            _Generator.MarkLabel(_Done);
        }
    }
}
