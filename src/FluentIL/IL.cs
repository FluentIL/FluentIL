using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace FluentIL
{
    public class IL
    {
        public static DynamicMethodBody NewMethod
            (Type returnType, params Type[] parameterTypes)
        {
            DynamicMethodInfo result = new DynamicMethodInfo();

            foreach (var param in parameterTypes)
                result.WithParameter(param);

            result.Returns(returnType);

            return result.Body;
        }

        public static DynamicMethodInfo NewMethod()
        {
            return new DynamicMethodInfo();
        }

        public static int EnsureLimits(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

    }
}
