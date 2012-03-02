using System;
using System.Linq;
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
        private int labelCountField;

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
            var value = (int) typeof (Label)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic) 
                .First()
                .GetValue(label);

            ProcessInstruction(labelsField[value].LabeledInstruction);
        }

        protected override Label OnDefineLabel()
        {
            labelsField.Add(new LabelInfo());
            labelsField[labelCountField].LabeledInstruction = 
                ilProcessorField.Create(OpCodes.Nop);

            return (Label)typeof(Label).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null, new Type[] { typeof(int) }, null
                ).Invoke(new object[] { labelCountField++ });
        }

        readonly List<LabelInfo> labelsField = new List<LabelInfo>();

        class LabelInfo
        {
            public Instruction LabeledInstruction { get; set; }
        }

        protected override void OnEmit(OpCode opcode)
        {
            ProcessInstruction(ilProcessorField.Create(opcode.ToCecil()));
        }

        protected override void OnEmit(OpCode opcode, string arg)
        {
            ProcessInstruction(ilProcessorField.Create(opcode.ToCecil(), arg));
        }

        protected override void OnEmit(OpCode opcode, int arg)
        {
            ProcessInstruction(ilProcessorField.Create(opcode.ToCecil(), arg));
        }

        protected override void OnEmit(OpCode opcode, double arg)
        {
            ProcessInstruction(ilProcessorField.Create(opcode.ToCecil(), arg));
        }

        protected override void OnEmit(OpCode opcode, Label arg)
        {
            var value = (int)typeof(Label)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .First()
                .GetValue(arg);

            ProcessInstruction(
                ilProcessorField.Create(
                    opcode.ToCecil(), 
                    labelsField[value].LabeledInstruction
                    ));
        }

        protected override void OnEmit(OpCode opcode, MethodInfo arg)
        {
            var reference = assemblyDefinitionField.MainModule.Import(arg);
            ProcessInstruction(ilProcessorField.Create(opcode.ToCecil(), reference));
        }

        protected override void OnEmit(OpCode opcode, ConstructorInfo arg)
        {
            var reference = assemblyDefinitionField.MainModule.Import(arg);
            ProcessInstruction(ilProcessorField.Create(opcode.ToCecil(), reference));
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

            ProcessInstruction(ilProcessorField.Create(opcode.ToCecil(), reference));
        }

        private void ProcessInstruction(Instruction instruction)
        {
            continuationField(instruction);
        }
    }

   
}