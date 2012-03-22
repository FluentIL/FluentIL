using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil.Cil;
using FluentIL.Emitters;
using Mono.Cecil;
using FluentIL.Cecil.Emitters;
using FluentIL.Infos;
using System;

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

            var dinfo = new DynamicMethodInfo(emitter);
            that.LoadArgsTo(dinfo);

            return dinfo.Body;
        }

        public static DynamicMethodBody InsertBeforeRet
            (this MethodDefinition that)
        {
            ILProcessor worker = that.Body.GetILProcessor();
            var aggregator = new EmittersAggregator();
            foreach (var instruction in worker.Body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Ret)
                {
                    Instruction instruction1 = instruction;
                    var emitter = new CecilILEmitter(
                        that.Module.Assembly,
                        worker,
                        inst => worker.InsertBefore(instruction1, inst));
                    aggregator.Emitters.Add(emitter);
                }
            }
            var dinfo = new DynamicMethodInfo(aggregator);
            that.LoadArgsTo(dinfo);

            return dinfo.Body;
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

            var dinfo = new DynamicMethodInfo(emitter);
            that.LoadArgsTo(dinfo);

            return dinfo.Body;
        }

        private static void LoadArgsTo(this MethodDefinition that, DynamicMethodInfo info)
        {
            if (!that.IsStatic)
                info.WithParameter<int>("$this");

            foreach (var arg in that.Parameters)
            {
                info.WithParameter(Type.GetType(arg.ParameterType.FullName), arg.Name);
            }
        }
    }
}