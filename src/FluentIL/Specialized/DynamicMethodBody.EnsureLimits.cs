// ReSharper disable CheckNamespace
namespace FluentIL
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody EnsureLimits(Number min, Number max)
        {
            return Dup()
                .Emit(min)
                .Iflt()
                .Pop()
                .Emit(min)
                .Else()
                .Dup()
                .Emit(max)
                .Ifgt()
                .Pop()
                .Emit(max)
                .EndIf()
                .EndIf();
        }
    }
}
