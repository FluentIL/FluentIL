using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace FluentIL.Infos
{
    public class DynamicAssemblyInfo
    {
        private readonly ModuleBuilder _moduleBuilder;
        private readonly AssemblyBuilder _assemblyBuilder;

        public DynamicAssemblyInfo(string name)
        {
            var assemblyName = new AssemblyName(
                name
                );

            _assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.RunAndSave
                );

            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(
                _assemblyBuilder.GetName().Name,
                false
                );
        }

        public DynamicTypeInfo WithType(string typeName)
        {
            return new DynamicTypeInfo(typeName, _moduleBuilder);
        }

        public void Save(string assemblyFileName)
        {
            _assemblyBuilder.Save(assemblyFileName);
        }
    }
}
