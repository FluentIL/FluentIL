// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody AbsR8()
        {
            return Dup()
                .Iflt(0.0)
                .Neg()
                .EndIf();
        }

        public DynamicMethodBody AbsI4()
        {
            return Dup()
                .Iflt(0)
                .Neg()
                .EndIf();
        }
    }
}