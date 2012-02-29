using System;
using FluentIL.Numbers;

// ReSharper disable CheckNamespace

namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        private const double EPSILON = 0.001;

        private void MultipleOperations(Func<DynamicMethodBody> action, params Number[] args)
        {
            Emit(args);
            if (args.Length == 1)
                action();
            else
                for (int i = 0; i < args.Length - 1; i++)
                    action();
        }

        // ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Rem(params Number[] args)
            // ReSharper restore MethodOverloadWithOptionalParameter
        {
            if (args == null) throw new ArgumentNullException("args");
            MultipleOperations(Rem, args);
            return this;
        }


        // ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Add(params Number[] args)
            // ReSharper restore MethodOverloadWithOptionalParameter
        {
            if (args == null) throw new ArgumentNullException("args");
            MultipleOperations(Add, args);
            return this;
        }

        public DynamicMethodBody Add(Number arg)
        {
            return Emit(arg).Add();
        }

        // ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Mul(params Number[] args)
            // ReSharper restore MethodOverloadWithOptionalParameter
        {
            if (args.Length == 1 && args[0] is ConstantDoubleNumber)
            {
                var constantDoubleNumber = args[0] as ConstantDoubleNumber;
                // ReSharper disable PossibleNullReferenceException
                double factor = constantDoubleNumber.Value;
                // ReSharper restore PossibleNullReferenceException
                if (Math.Abs(factor - 1) < EPSILON)
                    return this;
                if (Math.Abs(factor - -1) < EPSILON)
                    return Neg();
                return
                    LdcR8(factor).Mul();
            }

            MultipleOperations(Mul, args);
            return this;
        }


        // ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Div(params Number[] args)
            // ReSharper restore MethodOverloadWithOptionalParameter
        {
            MultipleOperations(Div, args);
            return this;
        }


        // ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Sub(params Number[] args)
            // ReSharper restore MethodOverloadWithOptionalParameter
        {
            MultipleOperations(Sub, args);
            return this;
        }
    }
}