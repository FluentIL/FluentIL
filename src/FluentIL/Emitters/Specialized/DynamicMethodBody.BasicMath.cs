using System;
using FluentIL.Numbers;

// ReSharper disable CheckNamespace

namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        private const double Epsilon = 0.001;

        private void MultipleOperations(Func<DynamicMethodBody> action, params Number[] args)
        {
            Emit(args);
            if (args.Length == 1)
                action();
            else
                for (var i = 0; i < args.Length - 1; i++)
                    action();
        }

        // ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Rem(params Number[] args)
            // ReSharper restore MethodOverloadWithOptionalParameter
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            MultipleOperations(Rem, args);
            return this;
        }


        // ReSharper disable MethodOverloadWithOptionalParameter
        public DynamicMethodBody Add(params Number[] args)
            // ReSharper restore MethodOverloadWithOptionalParameter
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
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
                var constantDoubleNumber = (ConstantDoubleNumber) args[0];
                // ReSharper disable PossibleNullReferenceException
                var factor = constantDoubleNumber.Value;
                // ReSharper restore PossibleNullReferenceException
                if (Math.Abs(factor - 1) < Epsilon)
                    return this;
                return Math.Abs(factor - -1) < Epsilon ? Neg() : LdcR8(factor).Mul();
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