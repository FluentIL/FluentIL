using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

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

            ModifyDoSomethingMethod(type);
            ModifyAddMethod(assembly, type);

            assembly.Write("ConsoleProgramThatWillBeChanged.Patched.exe");
        }

        private static void ModifyDoSomethingMethod(TypeDefinition type)
        {
            MethodDefinition method = type.Methods
                .First(m => m.Name == "DoSomething");

            ILProcessor worker = method.Body.GetILProcessor();

            worker.Body.Instructions.Clear();

            Instruction l1 = worker.Create(OpCodes.Ldstr,
                "Hello World from modified program");
            Instruction l2 = worker.Create(OpCodes.Ret);

            worker.Append(l1);
            worker.Append(l2);
        }

        private static void ModifyAddMethod(
            AssemblyDefinition assembly,
            TypeDefinition type
            )
        {
            MethodDefinition method = type.Methods
                .First(m => m.Name == "Add");

            ILProcessor worker = method.Body.GetILProcessor();

            MethodInfo minfo = typeof(Console).GetMethod(
                "WriteLine",
                new[] { typeof(string), typeof(int) });
            MethodReference writeLine = assembly.MainModule.Import(minfo);

            Instruction firstInstruction = worker.Body.Instructions[0];

            var l = new Instruction[8];

            l[0] = worker.Create(OpCodes.Ldstr,
                                 "Value of First Parameter is {0}");
            l[1] = worker.Create(OpCodes.Ldarg_0);
            l[2] = worker.Create(OpCodes.Box,
                                 assembly.MainModule.TypeSystem.Int32);
            l[3] = worker.Create(OpCodes.Call, writeLine);

            l[4] = worker.Create(OpCodes.Ldstr,
                                 "Value of Second Parameter is {0}");
            l[5] = worker.Create(OpCodes.Ldarg_1);
            l[6] = worker.Create(OpCodes.Box,
                                 assembly.MainModule.TypeSystem.Int32);

            l[7] = worker.Create(OpCodes.Call, writeLine);

            for (int i = 0; i <= 7; i++)
            {
                worker.InsertBefore(firstInstruction, l[i]);
            }
        }
    }
}