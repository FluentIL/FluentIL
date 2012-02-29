using System;

namespace ConsoleProgramThatWillBeChanged
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(DoSomething());
            Console.WriteLine(Add(4, 2));
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
    }
}