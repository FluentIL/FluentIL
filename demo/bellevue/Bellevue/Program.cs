using System;
using System.IO;
using Bellevue.Parser;
using FluentIL;
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

            var body = main.Returns(typeof (void));
            foreach (var block in TextParser.Parse(File.ReadAllText(input)))
            {
                string s = null;
                string v = null;
                if (block.TryLiteral(ref s))
                {
                    body.Write(s);
                }
                else if (block.TryFormula(ref s))
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        ParseResult result;
                        body
                            .Parse(s, out result)
                            .Write(result.ExpressionType);
                    }
                }
                else if (block.TryAssignment(ref v, ref s))
                {
                    ParseResult result;
                    body.Parse(s, out result);

                    if (body.GetVariableIndex(v) == -1)
                    {
                        main.WithVariable(result.ExpressionType, v);
                        main.GetILEmitter().DeclareLocal(result.ExpressionType);
                    }
                    body.Stloc(v);
                }
            }
            body.Ret();

            assembly.SetEntryPoint(main);
            assembly.Save();
        }
    }
}
