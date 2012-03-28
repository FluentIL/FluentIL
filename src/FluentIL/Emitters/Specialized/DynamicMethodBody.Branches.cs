using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentIL.ExpressionParser;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody IfEmptyString(bool not)
        {
            FieldInfo stringEmpty = typeof (string).GetField("Empty");
            MethodInfo stringOpEqualityMethod = typeof (string).GetMethod(
                "op_Equality", new[] {typeof (string), typeof (string)});

            var emitter = new IfEmitter(this);
            ifEmittersField.Push(emitter);
            Ldsfld(stringEmpty)
                .Call(stringOpEqualityMethod);

            emitter.EmitBranch(not);
            return this;
        }

        public DynamicMethodBody IfEmptyString()
        {
            return IfEmptyString(false);
        }

        public DynamicMethodBody IfNotEmptyString()
        {
            return IfEmptyString(true);
        }

        public DynamicMethodBody IfNull(bool not)
        {
            var emitter = new IfEmitter(this);
            ifEmittersField.Push(emitter);
            emitter.EmitBranch(!not);
            return this;
        }

        public DynamicMethodBody IfNull(
            bool not,
            Action<DynamicMethodBody> @then
            )
        {
            IfNull(not);
            @then(this);
            EndIf();

            return this;
        }


        public DynamicMethodBody IfNull(
            bool not,
            Action<DynamicMethodBody> @then,
            Action<DynamicMethodBody> @else
            )
        {
            IfNull(not);
            @then(this);
            Else();
            @else(this);
            EndIf();

            return this;
        }

        public DynamicMethodBody IfNull()
        {
            return IfNull(false);
        }

        public DynamicMethodBody IfNull(
            Action<DynamicMethodBody> @then
            )
        {
            return IfNull(false, @then);
        }

        public DynamicMethodBody IfNull(
            Action<DynamicMethodBody> @then,
            Action<DynamicMethodBody> @else
            )
        {
            return IfNull(false, @then, @else);
        }

        public DynamicMethodBody IfNotNull()
        {
            return IfNull(true);
        }

        public DynamicMethodBody IfNotNull(
            Action<DynamicMethodBody> @then
            )
        {
            return IfNull(true, @then);
        }

        public DynamicMethodBody IfNotNull(
            Action<DynamicMethodBody> @then,
            Action<DynamicMethodBody> @else
            )
        {
            return IfNull(true, @then, @else);
        }

        public DynamicMethodBody If(Expression expression)
        {
            var emitter = new IfEmitter(this);
            ifEmittersField.Push(emitter);
            Expression(expression);
            emitter.EmitBranch();
            return this;
        }

        public DynamicMethodBody If(
            Expression expression,
            Action<DynamicMethodBody> @then
            )
        {
            If(expression);
            @then(this);
            EndIf();

            return this;
        }

        public DynamicMethodBody If(
            Expression expression,
            Action<DynamicMethodBody> @then,
            Action<DynamicMethodBody> @else
            )
        {
            If(expression);
            @then(this);
            Else();
            @else(this);
            EndIf();

            return this;
        }

        public DynamicMethodBody If(string expression)
        {
            var emitter = new IfEmitter(this);
            ifEmittersField.Push(emitter);
            Parser.Parse(expression, this);
            emitter.EmitBranch();
            return this;
        }

        public DynamicMethodBody If(
            string expression, 
            Action<DynamicMethodBody> @then
            )
        {
            If(expression);
                @then(this);
            EndIf();

            return this;
        }

        public DynamicMethodBody If(
            string expression, 
            Action<DynamicMethodBody> @then,
            Action<DynamicMethodBody> @else
            )
        {
            If(expression);
                @then(this);
            Else();
                @else(this);
            EndIf();

            return this;
        }
    }
}