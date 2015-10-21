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
        private readonly AssemblyDefinition _assemblyDefinitionField;
        private readonly ILProcessor _ilProcessorField;
        private readonly Action<Instruction> _continuationField;
        private int _labelCountField;

        public CecilILEmitter(
            AssemblyDefinition assemblyDefinition,
            ILProcessor ilProcessor,
            Action<Instruction> continuation
            )
        {
            _assemblyDefinitionField = assemblyDefinition;
            _ilProcessorField = ilProcessor;
            _continuationField = continuation;
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

            ProcessInstruction(_labelsField[value].LabeledInstruction);
        }

        protected override Label OnDefineLabel()
        {
            _labelsField.Add(new LabelInfo());
            _labelsField[_labelCountField].LabeledInstruction = 
                _ilProcessorField.Create(OpCodes.Nop);

            return (Label)typeof(Label).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null, new[] { typeof(int) }, null
                ).Invoke(new object[] { _labelCountField++ });
        }

        readonly List<LabelInfo> _labelsField = new List<LabelInfo>();

        class LabelInfo
        {
            public Instruction LabeledInstruction { get; set; }
        }

        protected override void OnEmit(OpCode opcode)
        {
            ProcessInstruction(_ilProcessorField.Create(opcode.ToCecil()));
        }

        protected override void OnEmit(OpCode opcode, string arg)
        {
            ProcessInstruction(_ilProcessorField.Create(opcode.ToCecil(), arg));
        }

        protected override void OnEmit(OpCode opcode, int arg)
        {
            ProcessInstruction(_ilProcessorField.Create(opcode.ToCecil(), arg));
        }

        protected override void OnEmit(OpCode opcode, double arg)
        {
            ProcessInstruction(_ilProcessorField.Create(opcode.ToCecil(), arg));
        }

        protected override void OnEmit(OpCode opcode, Label arg)
        {
            var value = (int)typeof(Label)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .First()
                .GetValue(arg);

            ProcessInstruction(
                _ilProcessorField.Create(
                    opcode.ToCecil(), 
                    _labelsField[value].LabeledInstruction
                    ));
        }

        protected override void OnEmit(OpCode opcode, MethodInfo arg)
        {
            var reference = _assemblyDefinitionField.MainModule.Import(arg);
            ProcessInstruction(_ilProcessorField.Create(opcode.ToCecil(), reference));
        }

        protected override void OnEmit(OpCode opcode, ConstructorInfo arg)
        {
            var reference = _assemblyDefinitionField.MainModule.Import(arg);
            ProcessInstruction(_ilProcessorField.Create(opcode.ToCecil(), reference));
        }

        protected override void OnEmit(OpCode opcode, FieldInfo arg)
        {
            throw new NotImplementedException();
        }

        protected override void OnEmit(OpCode opcode, Type arg)
        {
            var ts = _assemblyDefinitionField.MainModule.TypeSystem;
            TypeReference reference;
            if (arg == typeof(int))
                reference = ts.Int32;
            else 
                throw  new NotSupportedException();

            ProcessInstruction(_ilProcessorField.Create(opcode.ToCecil(), reference));
        }

        private void ProcessInstruction(Instruction instruction)
        {
            _continuationField(instruction);
        }
    }

   
}