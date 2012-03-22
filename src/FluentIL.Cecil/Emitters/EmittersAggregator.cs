using System;
using System.Collections.Generic;
using System.Reflection;
using FluentIL.Emitters;
using System.Reflection.Emit;

namespace FluentIL.Cecil.Emitters
{
    public class EmittersAggregator : ILEmitter
    {
        readonly List<ILEmitter> emittersField = new List<ILEmitter>();

        public List<ILEmitter> Emitters
        {
            get { return emittersField;  }
        }

        protected override void OnDeclareLocal(Type type)
        {
            foreach (var ilEmitter in emittersField)
                ilEmitter.DeclareLocal(type);
        }

        protected override void OnMarkLabel(Label label)
        {
            foreach (var ilEmitter in emittersField)
                ilEmitter.MarkLabel(label);
        }

        protected override Label OnDefineLabel()
        {
            var result = new Label();
            foreach (var ilEmitter in emittersField)
                result = ilEmitter.DefineLabel();
            return result;
        }

        protected override void OnEmit(OpCode opcode)
        {
            foreach (var ilEmitter in emittersField)
                ilEmitter.Emit(opcode);
        }

        protected override void OnEmit(OpCode opcode, string arg)
        {
            foreach (var ilEmitter in emittersField)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, int arg)
        {
            foreach (var ilEmitter in emittersField)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, double arg)
        {
            foreach (var ilEmitter in emittersField)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, Label arg)
        {
            foreach (var ilEmitter in emittersField)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, MethodInfo arg)
        {
            foreach (var ilEmitter in emittersField)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, ConstructorInfo arg)
        {
            foreach (var ilEmitter in emittersField)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, FieldInfo arg)
        {
            foreach (var ilEmitter in emittersField)
                ilEmitter.Emit(opcode, arg);
        }

        protected override void OnEmit(OpCode opcode, Type arg)
        {
            foreach (var ilEmitter in emittersField)
                ilEmitter.Emit(opcode, arg);
        }
    }
}
