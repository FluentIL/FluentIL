using FluentIL.Metaprogramming;
using System;

namespace ConsoleApplicationThatWillBeChanged
{
    internal class Program
    {
        private static void Main()
        {
            try
            {
                MethodThatWillBeChanged("First Call");
                MethodThatWillBeChanged(null);
            }
            catch 
            {
                Console.WriteLine("An exception was catched.");
            }
        }

        public static void MethodThatWillBeChanged(
            [NotNull] string a
            )
        {
            Console.WriteLine("Value of the parameter is \"{0}\"", a);
        }
    }
}