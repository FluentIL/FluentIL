using System;
using System.Reflection.Emit;
using FluentIL.Infos;

namespace FluentIL.Emitters
{
    public partial class DynamicMethodBody
    {
        private readonly IDynamicMethodInfo _methodInfoField;

        internal DynamicMethodBody(IDynamicMethodInfo methodInfo)
        {
            _methodInfoField = methodInfo;
        }

        public DynamicMethod AsDynamicMethod => _methodInfoField.AsDynamicMethod;
        public Type AsType => _methodInfoField.DynamicTypeInfo.AsType;


        public DynamicMethodInfo WithMethod(string methodName)
        {
            return _methodInfoField.DynamicTypeInfo.WithMethod(methodName);
        }

        public object Invoke(params object[] args)
        {
            return _methodInfoField.AsDynamicMethod.Invoke(null, args);
        }

        #region static

        public static implicit operator DynamicMethod(DynamicMethodBody body)
        {
            return body._methodInfoField.AsDynamicMethod;
        }

        #endregion
    }
}