using System.Reflection.Emit;
using FluentIL.Numbers;

namespace FluentIL
{
    internal struct ForInfo
    {
        public ForInfo(string variable, Number from, Number to, int step,
                       Label beginLabel, Label comparasionLabel) :
                           this()
        {
            Variable = variable;
            From = from;
            To = to;
            Step = step;
            BeginLabel = beginLabel;
            ComparasionLabel = comparasionLabel;
        }

        public string Variable { get; private set; }
        public Number From { get; private set; }
        public Number To { get; private set; }
        public int Step { get; private set; }
        public Label BeginLabel { get; private set; }
        public Label ComparasionLabel { get; private set; }
    }
}