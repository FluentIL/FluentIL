using FluentIL.Emitters;

namespace FluentIL.Numbers
{
    public class ConstantInt32Number : Number
    {
        public ConstantInt32Number(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }

        public override void Emit(DynamicMethodBody generator)
        {
            generator.Ldc(Value);
        }
    }
}