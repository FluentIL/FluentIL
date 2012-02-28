using Cloning;
using System;
using System.Diagnostics;
namespace CloningConsole
{
    public class Person : ICloneable
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public object Clone()
        {
            return new Person
                       {
                           Age = this.Age,
                           Name = this.Name
                       };
        }
    }



    internal class Program
    {
        private static void Main()
        {
            var p = new Person {Name = "Doe, John", Age = 30};
            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < 500000; i++)
            {
                var p2 = p.Clone();
            }
            sw.Stop();
            Console.WriteLine("Conventional Cloning time: {0} ", sw.Elapsed);

            var cloner = p.GetShallowCloner();
            sw.Start();
            
            for (var i = 0; i < 500000; i++)
            {
                var p2 = cloner(p);
            }
            sw.Stop();
            Console.WriteLine("Clonner Cloning time: {0} ", sw.Elapsed);


            Console.ReadLine();

        }
    }
}