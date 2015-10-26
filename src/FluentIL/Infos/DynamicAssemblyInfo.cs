using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Threading;

namespace FluentIL.Infos
{
    public class DynamicAssemblyInfo
    {
        private readonly string _assemblyFileName;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly List<DynamicTypeInfo> _types = new List<DynamicTypeInfo>();

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
            var result = new DynamicTypeInfo(typeName, _moduleBuilder);
            _types.Add(result);
            return result;

        }

        public void SetEntryPoint(DynamicMethodInfo method) {
            _assemblyBuilder.SetEntryPoint(method.MethodBuilder);
        }

        public void Save()
        {
            _types.ForEach(t => t.Complete());
            _assemblyBuilder.Save(_assemblyFileName);
        }
    }
}
