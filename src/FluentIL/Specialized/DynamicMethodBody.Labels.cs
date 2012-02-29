using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace FluentIL
{
    partial class DynamicMethodBody
    {
        private readonly Dictionary<string, Label> labelsField = new Dictionary<string, Label>();

        public DynamicMethodBody MarkLabel(Label label)
        {
#if DEBUG
            Debug.Print("IL_{0}:", label.GetHashCode());
#endif

            infoField.GetILGenerator()
                .MarkLabel(label);

            return this;
        }

        public DynamicMethodBody MarkLabel(string label)
        {
            Label lbl = GetLabel(label);
#if DEBUG
            Debug.Print("IL_{0}:", lbl.GetHashCode());
#endif

            infoField.GetILGenerator()
                .MarkLabel(GetLabel(label));

            return this;
        }

        private Label GetLabel(string label)
        {
            if (!labelsField.ContainsKey(label))
                labelsField.Add(label, infoField.GetILGenerator().DefineLabel());

            return labelsField[label];
        }
    }
}
