using System;
using System.Reflection.Emit;
using FluentIL.Infos;

namespace FluentIL.Emitters
{
    public partial class DynamicMethodBody
    {
        private readonly DynamicMethodInfo methodInfoField;
        

        internal DynamicMethodBody(DynamicMethodInfo methodInfo)
        {
            methodInfoField = methodInfo;
        }

        public DynamicMethod AsDynamicMethod
        {
            get { return methodInfoField.AsDynamicMethod; }
        }

        public Type AsType
        {
            get { return methodInfoField.DynamicTypeInfo.AsType; }
        }


        public DynamicMethodInfo WithMethod(string methodName)
        {
            return methodInfoField.DynamicTypeInfo.WithMethod(methodName);
        }

        public object Invoke(params object[] args)
        {
            return methodInfoField.AsDynamicMethod.Invoke(null, args);
        }

        #region static

        public static implicit operator DynamicMethod(DynamicMethodBody body)
        {
            return body.methodInfoField;
        }

        public static implicit operator DynamicMethodInfo(DynamicMethodBody body)
        {
            return body.methodInfoField;
        }

        #endregion
    }
}