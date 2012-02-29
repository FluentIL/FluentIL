// ReSharper disable CheckNamespace
namespace FluentIL
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody AddToVar(string varname, Number constant)
        {
            return Ldloc(varname)
                .Add(constant)
                .Stloc(varname);
        }

        public DynamicMethodBody AddToVar(string varname)
        {
            return Ldloc(varname)
                .Add()
                .Stloc(varname);
        }
    }
}