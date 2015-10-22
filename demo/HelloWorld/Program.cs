using System.Reflection;
using FluentIL;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            var minfo = typeof(System.Console).GetMethod(
                "WriteLine",
                new[] { typeof(string) });

            var assembly = IL.NewAssembly("hello");
            var program = assembly.WithType("Program");
            //var program = IL.NewType("Program");

            program.WithMethod("Main")
                //.TurnOnAttributes(MethodAttributes.Static)
                //.TurnOffAttributes(MethodAttributes.Virtual)
                .Returns(typeof(void))
                    .Ldstr("Hello World")
                    .Call(minfo)
                    .Ret();

            var type = program.AsType;
            assembly.Save("hello.exe");
        }
    }
}
