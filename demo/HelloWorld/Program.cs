using FluentIL;

namespace HelloWorld
{
    class Program
    {
        static void Main()
        {
            var assembly = IL.NewAssembly("hello.exe");
            var program = assembly.WithType("Program");
            var main = program.WithStaticMethod("Main");

            main
                .Returns(typeof(void))
                .WriteLine("Hello World from FluentIL!")
                .Ret();

            assembly.SetEntryPoint(main);
            assembly.Save();
        }
    }
}
