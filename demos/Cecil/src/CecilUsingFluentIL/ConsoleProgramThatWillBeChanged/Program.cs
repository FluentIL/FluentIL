using System;

namespace ConsoleProgramThatWillBeChanged
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(DoSomething());
            Console.WriteLine(Add(4, 2));
            Console.WriteLine("Result should be 5 (original), but could be 6 (patched) version: {0}",
                DoOp(2, 3));

            Console.WriteLine("Result should be 13 (original), but could be 30 (patched) version: {0}",
                DoOp(10, 3));

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