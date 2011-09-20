using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using FluentIL.ExpressionParser;

namespace FluentIL
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody IfEmptyString(bool not)
        {
            var stringEmptyField = typeof(string).GetField("Empty");
            var stringOp_EqualityMethod = typeof(string).GetMethod(
                "op_Equality", new[] { typeof(string), typeof(string) });

            var emitter = new IfEmitter(this);
            _IfEmitters.Push(emitter);
            this
                .Ldsfld(stringEmptyField)
                .Call(stringOp_EqualityMethod);

            emitter.EmitBranch(not);
            return this;
        }

        public DynamicMethodBody IfEmptyString()
        {
            return this.IfEmptyString(false);
        }

        public DynamicMethodBody IfNotEmptyString()
        {
            return this.IfEmptyString(true);
        }

        public DynamicMethodBody IfNull(bool not)
        {
            var emitter = new IfEmitter(this);
            _IfEmitters.Push(emitter);
            emitter.EmitBranch(!not);
            return this;
        }

        public DynamicMethodBody IfNull()
        {
            return this.IfNull(false);
        }

        public DynamicMethodBody IfNotNull()
        {
            return this.IfNull(true);
        }

        public DynamicMethodBody If(Expression expression)
        {
            var emitter = new IfEmitter(this);
            _IfEmitters.Push(emitter);
            this.Expression(expression);
            emitter.EmitBranch(false);
            return this;
        }

        public DynamicMethodBody If(string expression)
        {
            var emitter = new IfEmitter(this);
            _IfEmitters.Push(emitter);
            Parser.Parse(expression, this);
            emitter.EmitBranch(false);
            return this;
        }
    }
}
