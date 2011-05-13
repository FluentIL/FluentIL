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
                EmitMethod(t, method, monitor);

            return CreateInstance<T>(t, concreteInstance, monitor);
        }

        private static T CreateInstance<T>(
            DynamicTypeInfo t,
            T concreteInstance,
            IProxyMonitor monitor = null
            )
        {
            var type = t.AsType;
            var result = (T)Activator.CreateInstance(type);

            var setupConcreteInstance = type.GetMethod("__SetConcreteInstance");
            setupConcreteInstance.Invoke(result, new object[] { concreteInstance });

            if (monitor != null)
            {
                var setupProxyMonitor = type.GetMethod("__SetProxyMonitor");
                setupProxyMonitor.Invoke(result, new object[] { monitor });
            }
            return result;
        }

        private static void EmitMethod(
            DynamicTypeInfo t, 
            MethodInfo method,
            IProxyMonitor monitor = null
            )
        {
            var ilmethod = t.WithMethod(method.Name);
            foreach (var param in method.GetParameters())
                ilmethod.WithParameter(
                    param.ParameterType,
                    param.Name
                    );

            if (monitor != null)
                ilmethod.WithVariable(method.ReturnType);
            
            var body = ilmethod
                .Returns(method.ReturnType);

            if (monitor != null)
                body
                    .Ldarg(0).Dup()
                    .Ldfld("__proxymonitor")
                    .Ldstr(method.Name)
                    ;

            body
                .Ldarg(0)
                .Ldfld("__concreteinstance");

            foreach (var param in method.GetParameters())
                body.Ldarg(param.Name);

            body
                .Call(method);

            if (monitor != null)
            {
                var afterExecuteMi = typeof(IProxyMonitor)
                    .GetMethod("AfterExecute");

                body
                    .Stloc(0).Ldloc(0)
                    .Call(afterExecuteMi)
                    .Ldloc(0);
            }

            body.Ret();
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
