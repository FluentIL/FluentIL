using System;
using System.Collections.Generic;
using System.Reflection;
using FluentIL.Emitters;
using System.Reflection.Emit;

namespace FluentIL.Cecil.Emitters
{
    public class EmittersAggregator : ILEmitter
    {
        public List<ILEmitter> Emitters { get; } = new List<ILEmitter>();

        protected override void OnDeclareLocal(Type type)
        {
            foreach (var ilEmitter in Emitters)
                ilEmitter.DeclareLocal(type);
        }

        protected override void OnMarkLabel(Label label)
        {
            foreach (var ilEmitter in Emitters)
                ilEmitter.MarkLabel(label);
        }

        protected override Label OnDefineLabel()
        {
            var result = new Label();
            foreach (var ilEmitter in Emitters)
                result = ilEmitter.DefineLabel();
            return result;
        }

        protected override void OnEmit(OpCode opcode)
        {
            foreach (var ilEmitter in Emitters)
                ilEmitter.Emit(opcode);
        }

        protected override void OnEmit(OpCode opcode, string arg)
        {
            foreach (var ilEmitter in Emitters)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, int arg)
        {
            foreach (var ilEmitter in Emitters)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, double arg)
        {
            foreach (var ilEmitter in Emitters)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, Label arg)
        {
            foreach (var ilEmitter in Emitters)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, MethodInfo arg)
        {
            foreach (var ilEmitter in Emitters)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, ConstructorInfo arg)
        {
            foreach (var ilEmitter in Emitters)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, FieldInfo arg)
        {
            foreach (var ilEmitter in Emitters)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, Type arg)
        {
            foreach (var ilEmitter in Emitters)
                ilEmitter.Emit(opcode, arg);
        }
    }
}
