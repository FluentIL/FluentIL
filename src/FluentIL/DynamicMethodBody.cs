using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using FluentIL.ExpressionInterpreter;
using FluentIL.ExpressionParser;

namespace FluentIL
{
    public partial class DynamicMethodBody
    {
        private readonly DynamicMethodInfo infoField;

        internal DynamicMethodBody(DynamicMethodInfo info)
        {
            infoField = info;
        }

        public DynamicMethod AsDynamicMethod
        {
            get { return infoField.AsDynamicMethod; }
        }

        public Type AsType
        {
            get { return infoField.DynamicTypeInfo.AsType; }
        }


        public DynamicMethodInfo WithMethod(string methodName)
        {
            return infoField.DynamicTypeInfo.WithMethod(methodName);
        }

        

        

        public object Invoke(params object[] args)
        {
            return infoField.AsDynamicMethod.Invoke(null, args);
        }

        #region static

        public static implicit operator DynamicMethod(DynamicMethodBody body)
        {
            return body.infoField;
        }

        public static implicit operator DynamicMethodInfo(DynamicMethodBody body)
        {
            return body.infoField;
        }

        #endregion
    }
}