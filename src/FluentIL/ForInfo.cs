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
        public int From { get; private set; }
        public int To { get; private set; }
        public int Step { get; private set; }
        public Label GoTo { get;  private set; }
        
        public ForInfo(string variable, int from, int to, int step, Label goTo) :
            this()
        {
            this.Variable = variable;
            this.From = from;
            this.To = to;
            this.Step = step;
            this.GoTo = goTo;
        }
    }
}
