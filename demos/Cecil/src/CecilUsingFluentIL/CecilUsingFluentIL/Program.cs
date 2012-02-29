using System;
using System.Linq;
using System.Reflection;
using FluentIL;
using FluentIL.Cecil.Emitters;
using Mono.Cecil;
using Mono.Cecil.Cil;
using _OpCodes = System.Reflection.Emit.OpCodes;

namespace CecilUsingFluentIL
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AssemblyDefinition assembly = AssemblyDefinition
                .ReadAssembly("ConsoleProgramThatWillBeChanged.exe");

            TypeDefinition type = assembly.MainModule.Types
                .First(t => t.Name == "Program");

            ModifyDoSomethingMethod(assembly, type);
            ModifyAddMethod(assembly, type);

            assembly.Write("ConsoleProgramThatWillBeChanged.Patched.exe");
        }

        private static void ModifyDoSomethingMethod(AssemblyDefinition assemblyDefinition, TypeDefinition type)
        {
            MethodDefinition method = type.Methods
                .First(m => m.Name == "DoSomething");

            ILProcessor worker = method.Body.GetILProcessor();
            var emitter = new CecilILEmitter(assemblyDefinition, worker, worker.Append);

            worker.Body.Instructions.Clear();

            IL.EmitTo(emitter)
                .Ldstr("Hello World from modified program")
                .Ret();
        }

        private static void ModifyAddMethod(
            AssemblyDefinition assembly,
            TypeDefinition type
            )
        {
            MethodDefinition method = type.Methods
                .First(m => m.Name == "Add");

            ILProcessor worker = method.Body.GetILProcessor();

            MethodInfo minfo = typeof (Console).GetMethod(
                "WriteLine",
                new[] {typeof (string), typeof (int)});

            var firstInstruction = worker.Body.Instructions[0];
            var emitter = new CecilILEmitter(
                assembly,
                worker,
                (inst) => worker.InsertBefore(firstInstruction, inst));

            IL.EmitTo(emitter)
                .Ldstr("Value of First Parameter is {0}")
                .Ldarg(0)
                .Box(typeof (int))
                .Call(minfo)
                .Ldstr("Value of Second Parameter is {0}")
                .Ldarg(1)
                .Box(typeof (int))
                .Call(minfo);
        }
    }
}