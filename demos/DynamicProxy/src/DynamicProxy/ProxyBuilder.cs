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
        public static T CreateProxy<T>(
            T concreteInstance, 
            IProxyMonitor monitor = null
            )
        {
            var t = IL.NewType().Implements<T>();

            EmitConcreteInstanceSupport<T>(t);
            EmitProxyMonitorSupport(t, monitor);

            foreach (var method in typeof(T).GetMethods())
                EmitMethod(t, method);

            return CreateInstance<T>(concreteInstance, t);
        }

        private static T CreateInstance<T>(
            T concreteInstance, 
            DynamicTypeInfo t
            )
        {
            var type = t.AsType;
            var setup = type.GetMethod("__SetConcreteInstance");
            var result = (T)Activator.CreateInstance(type);
            setup.Invoke(result, new object[] { concreteInstance });
            return result;
        }

        private static void EmitMethod(DynamicTypeInfo t, MethodInfo method)
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
                .Ldfld("__concreteinstance");

            foreach (var param in method.GetParameters())
                body.Ldarg(param.Name);

            body
                .Call(method)
                .Ret();
        }

        private static void EmitConcreteInstanceSupport<T>(DynamicTypeInfo t)
        {
            t
                .WithField("__concreteinstance", typeof(T))
                .WithMethod("__SetConcreteInstance")
                .WithParameter(typeof(T))
                .Returns(typeof(void))
                    .Ldarg(0)
                    .Ldarg(1)
                    .Stfld("__concreteinstance")
                    .Ret();
        }

        private static void EmitProxyMonitorSupport(
            DynamicTypeInfo t, 
            IProxyMonitor monitor
            )
        {
            if (monitor == null) return;

            t
                .WithField("__proxymonitor", typeof(IProxyMonitor))
                .WithMethod("__SetProxyMonitor")
                .WithParameter(typeof(IProxyMonitor))
                .Returns(typeof(void))
                    .Ldarg(0)
                    .Ldarg(1)
                    .Stfld("__proxymonitor")
                    .Ret();
        }
    }
}
