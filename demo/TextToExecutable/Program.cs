﻿using System;
using System.IO;
using FluentIL;

namespace TextToExecutable
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

            main
                .Returns(typeof(void))
                .Write(File.ReadAllText(input))
                .Ret();

            assembly.SetEntryPoint(main);
            assembly.Save();
        }
    }
}
