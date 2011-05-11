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
                EnsureTypeBuilder();
                return this.TypeBuilderField;
            }
        }

        void EnsureTypeBuilder()
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

                foreach (var field in _fields)
                {
                    field.FieldBuilder = this.TypeBuilderField.DefineField(
                        field.Name,
                        field.Type,
                        FieldAttributes.Private
                        );
                }
            }
        }

        List<Type> _interfaces = new List<Type>();
        public DynamicTypeInfo Implements<TInterface>()
        {
            _interfaces.Add(typeof(TInterface));
            return this;
        }

        List<DynamicFieldInfo> _fields = new List<DynamicFieldInfo>();
        public DynamicTypeInfo WithField(string fieldName, Type fieldType)
        {
            var value = new DynamicFieldInfo(
                this,
                fieldName, 
                fieldType);

            _fields.Add(
                value
                );

            if (this.TypeBuilderField != null)
            {
                value.FieldBuilder = this.TypeBuilderField.DefineField(
                            fieldName,
                            fieldType,
                            FieldAttributes.Private
                            );
            }
            return this;
        }

        public FieldInfo GetFieldInfo(string fieldName)
        {
            var result = _fields.First(f => f.Name.Equals(fieldName));
            if (result == null) return null;
            this.EnsureTypeBuilder();
            return result.FieldBuilder;
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
