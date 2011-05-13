using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProxy
{
    using FluentIL;
    using System.Reflection;

    public static class ProxyBuilder
    {
        public static T CreateProxy<T>(T instance)
        {
            var t = IL.NewType().Implements<T>();

            foreach (var method in typeof(T).GetMethods())
            {
                var ilmethod = t.WithMethod(method.Name);
                foreach (var param in method.GetParameters())
                    ilmethod.WithParameter(
                        param.ParameterType,
                        param.Name
                        );

                ilmethod.Returns(method.ReturnType)
                    .Throw<NotImplementedException>()
                    .Ret();
            }
            
            return (T)Activator.CreateInstance(t.AsType);
        }
    }
}
