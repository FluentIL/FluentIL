using FluentIL.Emitters;

namespace FluentIL.Numbers
{
    public class ConstantDoubleNumber : Number
    {
        public ConstantDoubleNumber(double value)
        {
            Value = value;
        }

        public double Value { get; private set; }

        public override void Emit(DynamicMethodBody generator)
        {
            generator.Ldc(Value);
        }
    }
}