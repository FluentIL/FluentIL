using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProxy
{
    public class ExpressionProxyMonitor
        : IProxyMonitor
    {
        public Action<string, object []> BeforeExecuteAction { get; set; }
        public void BeforeExecute(string methodName, object[] p)
        {
            if (this.BeforeExecuteAction == null)
                return;

            this.BeforeExecuteAction(methodName, p);
        }

        public Action<string, object> AfterExecuteAction { get; set; }
        public void AfterExecute(string methodName, object result)
        {
            if (this.AfterExecuteAction == null)
                return;

            this.AfterExecuteAction(methodName, result);
        }
    }
}
