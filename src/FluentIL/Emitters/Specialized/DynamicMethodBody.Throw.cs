using System;

// ReSharper disable CheckNamespace

namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Throw<TException>(params Type[] types)
            where TException : Exception
        {
            return Newobj<TException>(types)
                .Throw();
        }
    }
}