using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

namespace FluentIL
{
    public class DynamicTypeInfo
    {
        public string TypeName { get; private set; }

        public DynamicTypeInfo(string typeName)
        {
            this.TypeName = typeName;
        }

        TypeBuilder TypeBuilderField = null;
        public TypeBuilder TypeBuilder
        {
            get
            {
                if (this.TypeBuilderField == null)
                {
                    var assemblyName = new AssemblyName(
                        string.Format("__assembly__{0}", DateTime.Now.Millisecond)
                        );

                    var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                        assemblyName,
                        AssemblyBuilderAccess.RunAndSave
                        );

                    var moduleBuilder = assemblyBuilder.DefineDynamicModule(
                        assemblyBuilder.GetName().Name,
                        false
                        );

                    this.TypeBuilderField = moduleBuilder.DefineType(this.TypeName,
                        TypeAttributes.Public |
                        TypeAttributes.Class |
                        TypeAttributes.AutoClass |
                        TypeAttributes.AnsiClass |
                        TypeAttributes.BeforeFieldInit |
                        TypeAttributes.AutoLayout,
                        typeof(object),
                        _interfaces.ToArray()
                        );
                }

                return this.TypeBuilderField;
            }
        }

        List<Type> _interfaces = new List<Type>();
        public DynamicTypeInfo Implements<TInterface>()
        {
            _interfaces.Add(typeof(TInterface));
            return this;
        }


        public DynamicMethodInfo WithMethod(string methodName)
        {
            return new DynamicMethodInfo(this, methodName);
        }

        public Type AsType
        {
            get
            {
                return this.TypeBuilder.CreateType();
            }
        }
    }
}
