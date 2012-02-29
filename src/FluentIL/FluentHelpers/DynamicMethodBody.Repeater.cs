using System;

// ReSharper disable CheckNamespace
namespace FluentIL
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Repeater(int from, int to, int step,
                                          Action<int, DynamicMethodBody> action
            )
        {
            for (int i = from; i <= to; i += step)
                action(i, this);

            return this;
        }

        public DynamicMethodBody Repeater(int from, int to, int step,
                                          Func<int, DynamicMethodBody, bool> precondition,
                                          Action<int, DynamicMethodBody> action
            )
        {
            for (int i = from; i <= to; i += step)
                if (precondition(i, this))
                    action(i, this);

            return this;
        }
    }
}