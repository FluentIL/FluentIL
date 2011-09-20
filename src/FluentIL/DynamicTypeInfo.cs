using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;

namespace FluentIL
{
    public class DynamicTypeInfo
    {
        public string TypeName { get; private set; }

        public DynamicTypeInfo(string typeName)
        {
            this.TypeName = typeName;
#if DEBUG
            Debug.Print(".class {0}", this.TypeName);
#endif

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
#if DEBUG
            Debug.Print("implements {0}", typeof(TInterface));
#endif
            return this;
        }

        List<DynamicFieldInfo> _fields = new List<DynamicFieldInfo>();
        public DynamicTypeInfo WithField(string fieldName, Type fieldType)
        {
            var value = new DynamicFieldInfo(
                this,
                fieldName,
                fieldType);

#if DEBUG
            Debug.Print(".field ({0}) {1}", fieldType, fieldName);
#endif

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

        public DynamicTypeInfo WithAutoProperty(
            string propertyName,
            Type propertyType
            )
        {
            string fieldName = string.Format("_{0}", Guid.NewGuid());
            this.WithField(fieldName, propertyType);

            var property = this.TypeBuilder.DefineProperty(
                propertyName,
                PropertyAttributes.None,
                propertyType,
                new Type[] { }
                );

            var get_methodinfo = this.WithMethod(string.Format("get_{0}", propertyName))
                .TurnOnAttributes(MethodAttributes.RTSpecialName)
                .TurnOnAttributes(MethodAttributes.SpecialName);

            var get_method = get_methodinfo
                .Returns(propertyType)
                .Ldarg(0) // this;
                .Ldfld(fieldName)
                .Ret();

            property.SetGetMethod(get_methodinfo.MethodBuilder);

            var set_methodinfo = this.WithMethod(string.Format("set_{0}", propertyName))
                .TurnOnAttributes(MethodAttributes.RTSpecialName)
                .TurnOnAttributes(MethodAttributes.SpecialName);

            var set_method = set_methodinfo
                .WithParameter(propertyType, "value")
                .Returns(typeof(void))
                .Ldarg(0) // this;
                .Ldarg("value")
                .Stfld(fieldName)
                .Ret();

            property.SetSetMethod(set_methodinfo.MethodBuilder);

            return this;
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
