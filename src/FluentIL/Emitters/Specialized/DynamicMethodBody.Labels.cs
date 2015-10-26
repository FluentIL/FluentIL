using System;
using System.Collections.Generic;
using System.Reflection.Emit;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        private readonly Dictionary<string, Label> _labels = new Dictionary<string, Label>();

        public DynamicMethodBody MarkLabel(Label label)
        {
#if DEBUG
            Console.WriteLine("IL_{0}:", label.GetHashCode());
#endif

            _methodInfo.GetILEmitter()
                .MarkLabel(label);

            return this;
        }

        public DynamicMethodBody MarkLabel(string label)
        {
            var lbl = GetLabel(label);
#if DEBUG
            Console.WriteLine("IL_{0}:", lbl.GetHashCode());
#endif

            _methodInfo.GetILEmitter()
                .MarkLabel(GetLabel(label));

            return this;
        }

        private Label GetLabel(string label)
        {
            if (!_labels.ContainsKey(label))
                _labels.Add(label, _methodInfo.GetILEmitter().DefineLabel());

            return _labels[label];
        }
    }
}
