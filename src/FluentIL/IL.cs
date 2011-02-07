using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace FluentIL
{
    public class IL
    {
        public static DynamicMethodBody NewMethod(Type returnType, params Type[] parameterTypes)
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
    }
}
