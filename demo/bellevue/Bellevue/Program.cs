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
                if (block.IsLiteral)
                {
                    body.Write(((Tokens.Literal) block).Item);
                }
                else if (block.IsFormula)
                {
                    ParseResult result;
                    var formula = ((Tokens.Formula) block).Item;
                    body
                        .Parse(formula, out result)
                        .Write(result.ExpressionType);
                }
            }
            body.Ret();

            assembly.SetEntryPoint(main);
            assembly.Save();
        }

        public float Print(float input)
        {
            return input/2;
        }

        
    }
}
