using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace FluentIL
{
    struct ForInfo
    {
        public string Variable { get; private set; }
        public Number From { get; private set; }
        public Number To { get; private set; }
        public int Step { get; private set; }
        public Label BeginLabel { get;  private set; }
        public Label ComparasionLabel { get; private set; }

        
        public ForInfo(string variable, Number from, Number to, int step, 
            Label beginLabel, Label comparasionLabel) :
            this()
        {
            this.Variable = variable;
            this.From = from;
            this.To = to;
            this.Step = step;
            this.BeginLabel = beginLabel;
            this.ComparasionLabel = comparasionLabel;
        }
    }
}
