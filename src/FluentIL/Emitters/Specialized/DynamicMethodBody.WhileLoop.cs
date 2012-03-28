using System;
using System.Collections.Generic;
using FluentIL.Infos;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        private readonly Stack<WhileInfo> whilesField = new Stack<WhileInfo>();
        
        private DynamicMethodBody WhileOrUntil(string condition, bool isWhile)
        {
            var ilgen = methodInfoField.GetILEmitter();
            var beginLabel = ilgen.DefineLabel();
            var comparasionLabel = ilgen.DefineLabel();

            whilesField.Push(new WhileInfo(
                isWhile ? condition : "!(" + condition + ")",
                beginLabel,
                comparasionLabel));

            return Br(comparasionLabel)
                .MarkLabel(beginLabel);
        }
        
        public DynamicMethodBody While(string condition)
        {
            return WhileOrUntil(condition, true);
        }

        public DynamicMethodBody Until(string condition)
        {
            return WhileOrUntil(condition, false);
        }

        public DynamicMethodBody While(
            string condition,
            Action<DynamicMethodBody> @do
            )
        {
            While(condition);
                @do(this);
            Loop();
            return this;
        }

        public DynamicMethodBody Until(
            string condition,
            Action<DynamicMethodBody> @do
            )
        {
            Until(condition);
                @do(this);
            Loop();
            return this;
        }

        public DynamicMethodBody Loop()
        {
            var w = whilesField.Pop();
            return MarkLabel(w.ComparasionLabel)
                .If(w.Condition, m => m
                    .Br(w.BeginLabel)
                );
        }
    }
}