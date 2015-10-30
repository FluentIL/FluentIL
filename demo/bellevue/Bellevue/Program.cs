using System;
using System.Collections.Generic;
using System.IO;
using Bellevue.Parser;
using FluentIL;
using FluentIL.Infos;
using FluentIL.ExpressionParser;

namespace Bellevue
{
    class Program
    {
        static void Main(string[] args)
        {
            Action<string> print = Console.WriteLine;
            string input;
            string output;

            switch (args.Length)
            {
                case 1:
                    input = args[0];
                    output = Path.GetFileNameWithoutExtension(input) + ".exe";
                    break;
                case 2:
                    input = args[0];
                    output = args[1];
                    break;
                default:
                    print("Usage: t2e <text-file> [assembly-file]");
                    return;
            }

            if (!File.Exists(input))
            {
                print("Specified input file does not exist.");
                return;
            }


            var assembly = IL.NewAssembly(output);
            var program = assembly.WithType("Program");
            var main = program.WithStaticMethod("Main");

            var current = main;
            var methods = new Dictionary<string, DynamicMethodInfo>();
            foreach (var block in TextParser.Parse(File.ReadAllText(input)))
            {
                string s = null;
                string v = null;
                if (block.TryLiteral(ref s))
                {
                    current.Body.Write(s);
                }
                else if (block.TryFormula(ref s))
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        ParseResult result;
                        current.Body
                            .Parse(s, out result)
                            .Write(result.ExpressionType);
                    }
                }
                else if (block.TryAssignment(ref v, ref s))
                {
                    ParseResult result;
                    current.Body.Parse(s, out result);

                    if (current.Body.GetVariableIndex(v) == -1)
                    {
                        current.WithVariable(result.ExpressionType, v);
                        current.GetILEmitter().DeclareLocal(result.ExpressionType);
                    }
                    current.Body.Stloc(v);
                }
                else if (block.TryMethodCall(ref s))
                {
                    if (!methods.ContainsKey(s))
                    {
                        methods.Add(s, program.WithStaticMethod(s));
                    }
                    var minfo = methods[s];
                    minfo.GetILEmitter();
                    current.Body.Call(minfo.MethodBuilder);
                }
                else if (block.TrySection(ref s))
                {
                    if (!methods.ContainsKey(s))
                    {
                        methods.Add(s, program.WithStaticMethod(s));
                    }

                    current.Body.Ret();
                    current = methods[s];
                }
            }
            current.Body.Ret();

            assembly.SetEntryPoint(main);
            assembly.Save();
        }
    }
}
