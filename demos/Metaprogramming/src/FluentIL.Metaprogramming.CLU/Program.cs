using System;
using System.Linq;

using Mono.Cecil;

using FluentIL.Cecil;

namespace FluentIL.Metaprogramming.CLU
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: clu <assembly-file> <dest-assembly-file>");
                return;
            }

            var assembly = AssemblyDefinition.ReadAssembly(args[0]);
            var module = assembly.MainModule;

            var q = from type in module.Types
                    from method in type.Methods
                    from parameter in method.Parameters
                    where parameter.HasCustomAttributes
                    from attribute in parameter.CustomAttributes
                    where attribute.AttributeType.FullName ==
                          "FluentIL.Metaprogramming.NotNullAttribute"
                    select new {Method = method, Parameter = parameter};
 
            foreach (var item in q)
            {
                item.Method.InsertBefore()
                    .Ldarg(item.Parameter.Name)
                    .IfNull()
                        .Throw<ArgumentNullException>()
                    .EndIf();
            }

            assembly.Write(args[1]);
        }
    }
}