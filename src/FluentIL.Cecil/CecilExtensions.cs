using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil.Cil;
using FluentIL.Emitters;
using Mono.Cecil;
using FluentIL.Cecil.Emitters;

namespace FluentIL.Cecil
{
    public static class CecilExtensions
    {
        private static readonly Dictionary<string, OpCode>
            CecilOpcodes = new Dictionary<string, OpCode>();

        static CecilExtensions()
        {
            FieldInfo[] fields = typeof (OpCodes).GetFields();
            foreach (FieldInfo field in fields)
            {
                CecilOpcodes.Add(field.Name.ToLower().Replace('_', '.')
                                 , (OpCode) field.GetValue(null));
            }
        }

        public static OpCode ToCecil(
            this System.Reflection.Emit.OpCode that
            )
        {
            return
                CecilOpcodes[that.Name];
        }

        public static DynamicMethodBody InsertBefore
            (this MethodDefinition that)
        {
            ILProcessor worker = that.Body.GetILProcessor();

            var firstInstruction = worker.Body.Instructions[0];
            var emitter = new CecilILEmitter(
                that.Module.Assembly,
                worker,
                inst => worker.InsertBefore(firstInstruction, inst));

            return IL.EmitTo(emitter);
        }

        public static DynamicMethodBody ReplaceWith
            (this MethodDefinition that)
        {
            ILProcessor worker = that.Body.GetILProcessor();
            var emitter = new CecilILEmitter(
                that.Module.Assembly, 
                worker, 
                worker.Append
                );

            worker.Body.Instructions.Clear();

            return IL.EmitTo(emitter);
        }
    }
}