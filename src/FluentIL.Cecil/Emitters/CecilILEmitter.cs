using System;
using System.Reflection;
using System.Reflection.Emit;

using FluentIL.Emitters;
using Mono.Cecil;
using Mono.Cecil.Cil;
using OpCode = System.Reflection.Emit.OpCode;
using OpCodes = Mono.Cecil.Cil.OpCodes;
using System.Collections.Generic;

namespace FluentIL.Cecil.Emitters
{
    public class CecilILEmitter : ILEmitter
    {
        private readonly AssemblyDefinition assemblyDefinitionField;
        private readonly ILProcessor ilProcessorField;
        private readonly Action<Instruction> continuationField;

        public CecilILEmitter(
            AssemblyDefinition assemblyDefinition,
            ILProcessor ilProcessor,
            Action<Instruction> continuation
            )
        {
            assemblyDefinitionField = assemblyDefinition;
            ilProcessorField = ilProcessor;
            continuationField = continuation;
        }

        protected override void OnDeclareLocal(Type type)
        {
            throw new NotImplementedException();
        }

        protected override void OnMarkLabel(Label label)
        {
            throw new NotImplementedException();
        }

        protected override Label OnDefineLabel()
        {
            throw new NotImplementedException();
        }

        protected override void OnEmit(OpCode opcode)
        {
            continuationField(ilProcessorField.Create(opcode.ToCecil()));
        }

        protected override void OnEmit(OpCode opcode, string arg)
        {
            continuationField(ilProcessorField.Create(opcode.ToCecil(), arg));
        }

        protected override void OnEmit(OpCode opcode, int arg)
        {
            continuationField(ilProcessorField.Create(opcode.ToCecil(), arg));
        }

        protected override void OnEmit(OpCode opcode, double arg)
        {
            continuationField(ilProcessorField.Create(opcode.ToCecil(), arg));
        }

        protected override void OnEmit(OpCode opcode, Label arg)
        {
            throw new NotImplementedException();
        }

        protected override void OnEmit(OpCode opcode, MethodInfo arg)
        {
            var reference = assemblyDefinitionField.MainModule.Import(arg);
            continuationField(ilProcessorField.Create(opcode.ToCecil(), reference));
        }

        protected override void OnEmit(OpCode opcode, ConstructorInfo arg)
        {
            throw new NotImplementedException();
        }

        protected override void OnEmit(OpCode opcode, FieldInfo arg)
        {
            throw new NotImplementedException();
        }

        protected override void OnEmit(OpCode opcode, Type arg)
        {
            var ts = assemblyDefinitionField.MainModule.TypeSystem;
            TypeReference reference;
            if (arg == typeof(Int32))
                reference = ts.Int32;
            else 
                throw  new NotSupportedException();

            continuationField(ilProcessorField.Create(opcode.ToCecil(), reference));
        }
    }

    internal static class CecilExtensions
    {
        static CecilExtensions()
        {
            var fields = typeof (OpCodes).GetFields();
            foreach (var field in fields)
            {
                CecilOpcodes.Add(field.Name.ToLower().Replace('_', '.')
                    , (Mono.Cecil.Cil.OpCode) field.GetValue(null));
            }

        }

        static readonly Dictionary<string, Mono.Cecil.Cil.OpCode>
            CecilOpcodes = new Dictionary<string, Mono.Cecil.Cil.OpCode>();

        public static Mono.Cecil.Cil.OpCode ToCecil(
            this OpCode that
            )
        {
            return 
                   CecilOpcodes[that.Name];
        }
        
    }
}