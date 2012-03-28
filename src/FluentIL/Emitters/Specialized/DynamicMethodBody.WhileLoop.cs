using System;

// ReSharper disable CheckNamespace

namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody While(string condition)
        {
            throw new NotImplementedException();
        }

        public DynamicMethodBody Until(string condition)
        {
            throw new NotImplementedException();
        }

        public DynamicMethodBody While(
            string condition,
            Action<DynamicMethodBody> @do
            )
        {
            While(condition);
                @do(this);
            Loop();
            return this;
        }

        public DynamicMethodBody Until(
            string condition,
            Action<DynamicMethodBody> @do
            )
        {
            Until(condition);
                @do(this);
            Loop();
            return this;
        }

        public DynamicMethodBody Loop()
        {
            throw new NotImplementedException();
        }
    }
}