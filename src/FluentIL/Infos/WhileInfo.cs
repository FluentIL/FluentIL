using System.Reflection.Emit;

namespace FluentIL.Infos
{
    internal struct WhileInfo
    {
        public WhileInfo(string condition, Label beginLabel, Label comparasionLabel) :
            this()
        {
            Condition = condition;
            BeginLabel = beginLabel;
            ComparasionLabel = comparasionLabel;
        }

        public string Condition { get; private set; }
        public Label BeginLabel { get; private set; }
        public Label ComparasionLabel { get; private set; }
    }
}