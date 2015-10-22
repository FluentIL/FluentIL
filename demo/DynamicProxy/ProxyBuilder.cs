using System;
using System.Reflection;
using System.Reflection.Emit;
using FluentIL;
using FluentIL.Emitters;
using FluentIL.Infos;

namespace DynamicProxy
{
    public static class ProxyBuilder
    {
        public static T CreateProxy<T>(
            T concreteInstance, 
            Action<string, object []> beforeExecuteAction,
            Action<string, object> afterExecuteAction
            )
        {
            var monitor = new ExpressionProxyMonitor()
            {
                BeforeExecuteAction = beforeExecuteAction,
                AfterExecuteAction = afterExecuteAction
            };

            return CreateProxy(concreteInstance, monitor);
        }
        
        public static T CreateProxy<T>(
            T concreteInstance, 
            IProxyMonitor monitor = null
            )
        {
            return CreateInstance(
                IL.NewType().Implements<T>()
                    .EmitConcreteInstanceSupport<T>()
                    .EmitProxyMonitorSupport(condition: monitor != null)
                    .EmitMethods<T>(monitor),
                concreteInstance, monitor);
        }

        private static T CreateInstance<T>(
            this DynamicTypeInfo that,
            T concreteInstance,
            IProxyMonitor monitor = null
            )
        {
            var type = that.AsType;
            var result = (T)Activator.CreateInstance(type);

            var setupConcreteInstance = type.GetMethod("__SetConcreteInstance");
            setupConcreteInstance.Invoke(result, new object[] { concreteInstance });

            if (monitor == null) return result;

            var setupProxyMonitor = type.GetMethod("__SetProxyMonitor");
            setupProxyMonitor.Invoke(result, new object[] { monitor });
            return result;
        }

        private static DynamicTypeInfo EmitMethods<T>(
            this DynamicTypeInfo that,
            IProxyMonitor monitor
            )
        {
            foreach (var method in typeof(T).GetMethods())
                EmitMethod(that, method, monitor);
            return that;
        }

        private static void EmitMethod(
            DynamicTypeInfo t, 
            MethodInfo method,
            IProxyMonitor monitor = null
            )
        {
            EmitMethodSignature(t, method, monitor)
                .EmitBeforeExecuteCall(condition: monitor != null, method: method)
                .EmitConcreteMethodCall(method)
                .EmitAfterExecuteCall(condition: monitor != null, method: method)
                .Ret();
        }

        private static DynamicMethodBody EmitConcreteMethodCall(
            this DynamicMethodBody body,
            MethodInfo method )
        {
            var parameters = method.GetParameters();

            return body
                .Ldarg(0)
                .Ldfld("__concreteinstance")
                .Repeater(0, parameters.Length - 1, 1, (index, b) => b
                    .Ldarg(parameters[index].Name)
                    )
                .Call(method);
        }

        private static DynamicMethodBody EmitMethodSignature(DynamicTypeInfo t, MethodInfo method, IProxyMonitor monitor)
        {
            var ilmethod = t.WithMethod(method.Name);
            var parameters = method.GetParameters();

            foreach (var param in parameters)
                ilmethod.WithParameter(
                    param.ParameterType,
                    param.Name
                    );

            if (monitor == null)
                return ilmethod
                    .Returns(method.ReturnType);

            if (method.ReturnType != typeof(void))
                ilmethod.WithVariable(method.ReturnType);

            if (parameters.Length > 0)
                ilmethod.WithVariable(typeof(object[]), "parameters");

            return ilmethod
                .Returns(method.ReturnType);
        }

        private static DynamicMethodBody EmitBeforeExecuteCall(
            this DynamicMethodBody body,
            bool condition,
            MethodBase method
            )
        {
            if (!condition) return body;

            var beforeExecuteMi = typeof(IProxyMonitor)
                .GetMethod("BeforeExecute");

            var parameters = method.GetParameters();

            return body
                .Ldarg(0).Dup()
                .Ldfld("__proxymonitor")
                .Ldstr(method.Name)
                .Newarr(typeof(object), parameters.Length)
                .EmitIf( parameters.Length > 0, b => b
                    .Stloc("parameters")
                    .Repeater(0, parameters.Length - 1, 1, (index, b1) => b1
                        .Ldloc("parameters")
                        .Ldc(index)
                        .Ldarg((uint)(index + 1))
                        .Box(parameters[index].ParameterType)
                        .Emit(OpCodes.Stelem_Ref)
                        )
                    .Ldloc("parameters")
                    )
                .Call(beforeExecuteMi)
                //
                .Ldfld("__proxymonitor")
                .Ldstr(method.Name);
        }

        private static DynamicMethodBody EmitAfterExecuteCall(
            this DynamicMethodBody body,
            bool condition,
            MethodInfo method )
        {
            if (!condition) return body;
            var afterExecuteMi = typeof(IProxyMonitor)
                .GetMethod("AfterExecute");

            if (method.ReturnType != typeof(void))
                body
                    .Stloc(0)
                    .Ldloc(0)
                    .Box(method.ReturnType);
            else
                body.Ldnull();

            body
                .Call(afterExecuteMi);

            if (method.ReturnType != typeof(void))
                body.Ldloc(0);

            return body;
        }

        private static DynamicTypeInfo EmitConcreteInstanceSupport<T>(
            this DynamicTypeInfo that)
        {
            that
                .WithField("__concreteinstance", typeof(T))
                .WithMethod("__SetConcreteInstance")
                .WithParameter(typeof(T))
                .Returns(typeof(void))
                    .Ldarg(0)
                    .Ldarg(1)
                    .Stfld("__concreteinstance")
                    .Ret();
            return that;
        }

        private static DynamicTypeInfo EmitProxyMonitorSupport(
            this DynamicTypeInfo that, 
            bool condition
            )
        {
            if (!condition) return that;

            that
                .WithField("__proxymonitor", typeof(IProxyMonitor))
                .WithMethod("__SetProxyMonitor")
                .WithParameter(typeof(IProxyMonitor))
                .Returns(typeof(void))
                    .Ldarg(0)
                    .Ldarg(1)
                    .Stfld("__proxymonitor")
                    .Ret();

            return that;
        }
    }
}
