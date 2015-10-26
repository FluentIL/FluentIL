using System;
using System.Reflection.Emit;
using FluentIL.Infos;

namespace FluentIL.Emitters
{
    public partial class DynamicMethodBody
    {
        private readonly IDynamicMethodInfo _methodInfo;

        internal DynamicMethodBody(IDynamicMethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }

        public DynamicMethod AsDynamicMethod => _methodInfo.AsDynamicMethod;
        public Type AsType => _methodInfo.DynamicTypeInfo.AsType;


        public DynamicMethodInfo WithMethod(string methodName)
        {
            return _methodInfo.DynamicTypeInfo.WithMethod(methodName);
        }

        public DynamicMethodBody WriteLine(string message)
        {
            var minfo = typeof(Console).GetMethod(
                "WriteLine",
                new[] { typeof(string) });
            return Ldstr(message).Call(minfo);
        }

        public DynamicMethodBody Write(string message)
        {
            var minfo = typeof(Console).GetMethod(
                "Write",
                new[] { typeof(string) });
            return Ldstr(message).Call(minfo);
        }

        public object Invoke(params object[] args)
        {
            return _methodInfo.AsDynamicMethod.Invoke(null, args);
        }

        #region static

        public static implicit operator DynamicMethod(DynamicMethodBody body)
        {
            return body._methodInfo.AsDynamicMethod;
        }


        #endregion
    }
}