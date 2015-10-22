using System;

namespace DynamicProxy
{
    public class ExpressionProxyMonitor
        : IProxyMonitor
    {
        public Action<string, object []> BeforeExecuteAction { get; set; }
        public void BeforeExecute(string methodName, object[] p)
        {
            BeforeExecuteAction?.Invoke(methodName, p);
        }

        public Action<string, object> AfterExecuteAction { get; set; }
        public void AfterExecute(string methodName, object result)
        {
            AfterExecuteAction?.Invoke(methodName, result);
        }
    }
}
