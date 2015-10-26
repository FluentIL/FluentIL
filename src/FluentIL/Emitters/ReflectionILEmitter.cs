using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FluentIL.Emitters
{
    internal class ReflectionILEmitter : ILEmitter
    {
        private readonly ILGenerator _ilGenerator;
        public ReflectionILEmitter(ILGenerator ilGenerator)
        {
            _ilGenerator = ilGenerator;
        }

        protected override void OnEmit(OpCode opcode)
        {
            _ilGenerator.Emit(opcode);
        }

        protected override void OnEmit(OpCode opcode, string arg)
        {
            _ilGenerator.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, int arg)
        {
            _ilGenerator.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, double arg)
        {
            _ilGenerator.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, Label arg)
        {
            _ilGenerator.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, MethodInfo arg)
        {
            _ilGenerator.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, ConstructorInfo arg)
        {
            _ilGenerator.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, FieldInfo arg)
        {
            _ilGenerator.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, Type arg)
        {
            _ilGenerator.Emit(opcode, arg);
        }

        protected override Label OnDefineLabel()
        {
            return _ilGenerator.DefineLabel();
        }

        protected override void OnMarkLabel(Label label)
        {
            _ilGenerator.MarkLabel(label);
        }

        protected override void OnDeclareLocal(Type type)
        {
            _ilGenerator.DeclareLocal(type);
        }

        public Label BeginExceptionBlock()
        {
            return _ilGenerator.BeginExceptionBlock();
        }

        public void BeginCatchBlock(Type exceptionType)
        {
            _ilGenerator.BeginCatchBlock(exceptionType);
        }

        public void BeginFaultBlock()
        {
            _ilGenerator.BeginFaultBlock();
        }

        public void EndExceptionBlock()
        {
            _ilGenerator.EndExceptionBlock();
        }

        internal void BeginFinallyBlock()
        {
           _ilGenerator.BeginFinallyBlock();
        }
    }
}