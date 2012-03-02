using System;
using System.Linq;
using Mono.Cecil;

namespace FluentIL.Metaprogramming.CLU
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: clu <assembly-file>");
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
                Console.WriteLine("Method={0}, Arg={1}", 
                    item.Method.Name,
                    item.Parameter.Name
                    );
            }
        }
    }
}