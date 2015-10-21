using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FluentIL.Emitters
{
    internal class ReflectionILEmitter : ILEmitter
    {
        private readonly ILGenerator _ilGeneratorField;
        public ReflectionILEmitter(ILGenerator ilGenerator)
        {
            _ilGeneratorField = ilGenerator;
        }

        protected override void OnEmit(OpCode opcode)
        {
            _ilGeneratorField.Emit(opcode);
        }

        protected override void OnEmit(OpCode opcode, string arg)
        {
            _ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, int arg)
        {
            _ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, double arg)
        {
            _ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, Label arg)
        {
            _ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, MethodInfo arg)
        {
            _ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, ConstructorInfo arg)
        {
            _ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, FieldInfo arg)
        {
            _ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, Type arg)
        {
            _ilGeneratorField.Emit(opcode, arg);
        }

        protected override Label OnDefineLabel()
        {
            return _ilGeneratorField.DefineLabel();
        }

        protected override void OnMarkLabel(Label label)
        {
            _ilGeneratorField.MarkLabel(label);
        }

        protected override void OnDeclareLocal(Type type)
        {
            _ilGeneratorField.DeclareLocal(type);
        }

        public Label BeginExceptionBlock()
        {
            return _ilGeneratorField.BeginExceptionBlock();
        }

        public void BeginCatchBlock(Type exceptionType)
        {
            _ilGeneratorField.BeginCatchBlock(exceptionType);
        }

        public void BeginFaultBlock()
        {
            _ilGeneratorField.BeginFaultBlock();
        }

        public void EndExceptionBlock()
        {
            _ilGeneratorField.EndExceptionBlock();
        }

        internal void BeginFinallyBlock()
        {
           _ilGeneratorField.BeginFinallyBlock();
        }
    }
}