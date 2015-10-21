using System;
using System.Collections.Generic;
using System.Linq;
using FluentIL.Cecil.Emitters;
using FluentIL.Emitters;
using FluentIL.Infos;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace FluentIL.Cecil
{
    public static class CecilExtensions
    {
        private static readonly Dictionary<string, OpCode>
            CecilOpcodes = new Dictionary<string, OpCode>();

        static CecilExtensions()
        {
            var fields = typeof (OpCodes).GetFields();
            foreach (var field in fields)
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
            var worker = that.Body.GetILProcessor();

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
            var worker = that.Body.GetILProcessor();
            var aggregator = new EmittersAggregator();
            foreach (var emitter in worker.Body.Instructions.Where(instruction => instruction.OpCode == OpCodes.Ret).Select(instruction1 => new CecilILEmitter(
                that.Module.Assembly,
                worker,
                inst => worker.InsertBefore(instruction1, inst))))
            {
                aggregator.Emitters.Add(emitter);
            }
            var dinfo = new DynamicMethodInfo(aggregator);
            that.LoadArgsTo(dinfo);

            return dinfo.Body;
        }

        public static DynamicMethodBody ReplaceWith
            (this MethodDefinition that)
        {
            var worker = that.Body.GetILProcessor();
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

        public static DynamicMethodBody NewMethod
            (this TypeDefinition that, string methodName, MethodAttributes methodAttributes, Type returnType, AssemblyDefinition assembly)
        {
            var typeReference = assembly.MainModule.Import(returnType);
            
            var method = new MethodDefinition(methodName, methodAttributes, typeReference);
            
            var worker = method.Body.GetILProcessor();

            var emitter = new CecilILEmitter(
                assembly,
                worker,
                method.Body.Instructions.Add);

            var dinfo = new DynamicMethodInfo(emitter);
            method.LoadArgsTo(dinfo);

            that.Methods.Add(method);

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