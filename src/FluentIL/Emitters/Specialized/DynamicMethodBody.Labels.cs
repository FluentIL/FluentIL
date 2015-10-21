using System;
using System.Collections.Generic;
using System.Reflection.Emit;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        private readonly Dictionary<string, Label> _labelsField = new Dictionary<string, Label>();

        public DynamicMethodBody MarkLabel(Label label)
        {
#if DEBUG
            Console.WriteLine("IL_{0}:", label.GetHashCode());
#endif

            _methodInfoField.GetILEmitter()
                .MarkLabel(label);

            return this;
        }

        public DynamicMethodBody MarkLabel(string label)
        {
            var lbl = GetLabel(label);
#if DEBUG
            Console.WriteLine("IL_{0}:", lbl.GetHashCode());
#endif

            _methodInfoField.GetILEmitter()
                .MarkLabel(GetLabel(label));

            return this;
        }

        private Label GetLabel(string label)
        {
            if (!_labelsField.ContainsKey(label))
                _labelsField.Add(label, _methodInfoField.GetILEmitter().DefineLabel());

            return _labelsField[label];
        }
    }
}
