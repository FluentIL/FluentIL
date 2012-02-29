// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Ret(bool returnValue)
        {
            return Ldc(returnValue ? 1 : 0)
                .Ret();
        }
    }
}