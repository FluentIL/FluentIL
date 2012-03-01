using System;

namespace ConsoleProgramThatWillBeChanged
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(DoSomething());
            Console.WriteLine(Add(4, 2));
            Console.WriteLine("Result will be 5 in original and 6 in the patched version: {0}",
                DoOp(2, 3));
            Console.ReadLine();
        }

        public static string DoSomething()
        {
            string result = "Hello world from Original Code";
            Console.WriteLine(result);
            return result;
        }

        public static int Add(int a, int b)
        {
            return a + b;
        }

        public static int DoOp(int a, int b)
        {
            return a + b;
        }
    }
}