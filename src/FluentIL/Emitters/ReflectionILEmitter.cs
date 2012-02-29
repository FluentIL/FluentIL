using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FluentIL.Emitters
{
    internal class ReflectionILEmitter : ILEmitter
    {
        private readonly ILGenerator ilGeneratorField;
        public ReflectionILEmitter(ILGenerator ilGenerator)
        {
            ilGeneratorField = ilGenerator;
        }

        protected override void OnEmit(OpCode opcode)
        {
            ilGeneratorField.Emit(opcode);
        }

        protected override void OnEmit(OpCode opcode, string arg)
        {
            ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, int arg)
        {
            ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, double arg)
        {
            ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, Label arg)
        {
            ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, MethodInfo arg)
        {
            ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, ConstructorInfo arg)
        {
            ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, FieldInfo arg)
        {
            ilGeneratorField.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, Type arg)
        {
            ilGeneratorField.Emit(opcode, arg);
        }

        protected override Label OnDefineLabel()
        {
            return ilGeneratorField.DefineLabel();
        }

        protected override void OnMarkLabel(Label label)
        {
            ilGeneratorField.MarkLabel(label);
        }

        protected override void OnDeclareLocal(Type type)
        {
            ilGeneratorField.DeclareLocal(type);
        }
    }
}