using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FluentIL.Emitters
{
    public abstract class ILEmitter
    {
        public void DeclareLocal(Type type)
        {
            OnDeclareLocal(type);
        }

        protected abstract void OnDeclareLocal(Type type);

        public void MarkLabel(Label label)
        {
            OnMarkLabel(label);
        }

        protected abstract void OnMarkLabel(Label label);

        public Label DefineLabel()
        {
            return OnDefineLabel();
        }

        protected abstract Label OnDefineLabel();
        
        public void Emit(OpCode opcode)
        {
            OnEmit(opcode);
        }

        protected abstract void OnEmit(OpCode opcode);

        public void Emit(OpCode opcode, string arg)
        {
            OnEmit(opcode, arg);
        }

        protected abstract void OnEmit(OpCode opcode, string arg);

        public void Emit(OpCode opcode, int arg)
        {
            OnEmit(opcode, arg);
        }

        protected abstract void OnEmit(OpCode opcode, int arg);

        public void Emit(OpCode opcode, double arg)
        {
            OnEmit(opcode, arg);
        }

        protected abstract void OnEmit(OpCode opcode, double arg);

        public void Emit(OpCode opcode, Label arg)
        {
            OnEmit(opcode, arg);
        }

        protected abstract void OnEmit(OpCode opcode, Label arg);

        public void Emit(OpCode opcode, MethodInfo arg)
        {
            OnEmit(opcode, arg);
        }

        protected abstract void OnEmit(OpCode opcode, MethodInfo arg);

        public void Emit(OpCode opcode, ConstructorInfo arg)
        {
            OnEmit(opcode, arg);
        }

        protected abstract void OnEmit(OpCode opcode, ConstructorInfo arg);

        public void Emit(OpCode opcode, FieldInfo arg)
        {
            OnEmit(opcode, arg);
        }

        protected abstract void OnEmit(OpCode opcode, FieldInfo arg);

        public void Emit(OpCode opcode, Type arg)
        {
            OnEmit(opcode, arg);
        }

        protected abstract void OnEmit(OpCode opcode, Type arg);
    }
}