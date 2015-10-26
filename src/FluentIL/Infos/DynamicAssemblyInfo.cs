using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace FluentIL.Infos
{
    public class DynamicAssemblyInfo
    {
        private readonly string _assemblyFileName;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly AssemblyBuilder _assemblyBuilder;

        public DynamicAssemblyInfo(string assemblyFileName)
        {
            _assemblyFileName = assemblyFileName;
            var assemblyName = new AssemblyName(
                assemblyFileName
                );

            _assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.RunAndSave
                );

            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(
                _assemblyBuilder.GetName().Name,
                assemblyFileName,
                false
                );
        }

        public DynamicTypeInfo WithType(string typeName)
        {
            return new DynamicTypeInfo(typeName, _moduleBuilder);
        }

        public void Save()
        {
            _assemblyBuilder.Save(_assemblyFileName);
        }
    }
}
