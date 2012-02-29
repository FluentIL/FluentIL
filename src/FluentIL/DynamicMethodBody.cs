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

        public DynamicMethodBody Parse(string expression)
        {
            Parser.Parse(expression, this);
            return this;
        }


        public DynamicMethodBody Expression(Expression expression)
        {
            expression = new ExpressionSimplifierVisitor().Visit(expression);
            new ILEmitterVisitor(this).Visit(
                expression
                );
            return this;
        }

        public DynamicMethodBody Repeater(int from, int to, int step,
                                          Action<int, DynamicMethodBody> action
            )
        {
            for (int i = from; i <= to; i += step)
                action(i, this);

            return this;
        }

        public DynamicMethodBody Repeater(int from, int to, int step,
                                          Func<int, DynamicMethodBody, bool> precondition,
                                          Action<int, DynamicMethodBody> action
            )
        {
            for (int i = from; i <= to; i += step)
                if (precondition(i, this))
                    action(i, this);

            return this;
        }


        public DynamicMethodBody EmitIf(bool condition, Action<DynamicMethodBody> action)
        {
            if (condition)
                action(this);

            return this;
        }

        public object Invoke(params object[] args)
        {
            return infoField.AsDynamicMethod.Invoke(null, args);
        }

        #region extended Stloc

        

        #endregion

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