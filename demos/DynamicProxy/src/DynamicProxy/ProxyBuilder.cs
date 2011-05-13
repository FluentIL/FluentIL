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

            t
                .WithField("__concreteinstance", typeof(T))
                .WithMethod("__SetConcreteInstance")
                .WithParameter(typeof(T))
                .Returns(typeof(void))
                    .Ldarg(0)
                    .Ldarg(1)
                    .Stfld("__concreteinstance")
                    .Ret();
                ;

            foreach (var method in typeof(T).GetMethods())
            {
                var ilmethod = t.WithMethod(method.Name);
                foreach (var param in method.GetParameters())
                    ilmethod.WithParameter(
                        param.ParameterType,
                        param.Name
                        );

                var body = ilmethod.Returns(method.ReturnType);

                body
                    .Ldarg(0)
                    .Ldfld("__concreteinstance")
                    .Call(method)
                    .Ret();
            }

            var type = t.AsType;
            var setup = type.GetMethod("__SetConcreteInstance");
            var result = (T)Activator.CreateInstance(type);
            setup.Invoke(result, new object [] {instance});
            return result;
        }
    }
}
