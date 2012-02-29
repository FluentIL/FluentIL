using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using FluentIL.Emitters;

namespace FluentIL.Infos
{
    public class DynamicTypeInfo
    {
        private readonly List<DynamicFieldInfo> fieldsField = new List<DynamicFieldInfo>();
        private readonly List<Type> interfacesField = new List<Type>();
        private TypeBuilder typeBuilderField;

        public DynamicTypeInfo(string typeName)
        {
            TypeName = typeName;
#if DEBUG
            Debug.Print(".class {0}", TypeName);
#endif
        }

        public string TypeName { get; private set; }

        public TypeBuilder TypeBuilder
        {
            get
            {
                EnsureTypeBuilder();
                return typeBuilderField;
            }
        }

        public Type AsType
        {
            get { return TypeBuilder.CreateType(); }
        }

        private void EnsureTypeBuilder()
        {
            if (typeBuilderField == null)
            {
                var assemblyName = new AssemblyName(
                    string.Format("__assembly__{0}", DateTime.Now.Millisecond)
                    );

                AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                    assemblyName,
                    AssemblyBuilderAccess.RunAndSave
                    );

                ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(
                    assemblyBuilder.GetName().Name,
                    false
                    );

                typeBuilderField = moduleBuilder.DefineType(TypeName,
                                                            TypeAttributes.Public |
                                                            TypeAttributes.Class |
                                                            TypeAttributes.AutoClass |
                                                            TypeAttributes.AnsiClass |
                                                            TypeAttributes.BeforeFieldInit |
                                                            TypeAttributes.AutoLayout,
                                                            typeof (object),
                                                            interfacesField.ToArray()
                    );

                foreach (DynamicFieldInfo field in fieldsField)
                {
                    field.FieldBuilder = typeBuilderField.DefineField(
                        field.Name,
                        field.Type,
                        FieldAttributes.Private
                        );
                }
            }
        }

        public DynamicTypeInfo Implements<TInterface>()
        {
            interfacesField.Add(typeof (TInterface));
#if DEBUG
            Debug.Print("implements {0}", typeof (TInterface));
#endif
            return this;
        }

// ReSharper disable InconsistentNaming
        public DynamicTypeInfo WithField(string fieldName, Type fieldType)
// ReSharper restore InconsistentNaming
        {
            var value = new DynamicFieldInfo(
                this,
                fieldName,
                fieldType);

#if DEBUG
            Debug.Print(".field ({0}) {1}", fieldType, fieldName);
#endif

            fieldsField.Add(
                value
                );

            if (typeBuilderField != null)
            {
                value.FieldBuilder = typeBuilderField.DefineField(
                    fieldName,
                    fieldType,
                    FieldAttributes.Private
                    );
            }
            return this;
        }

        public FieldInfo GetFieldInfo(string fieldName)
        {
            DynamicFieldInfo result = fieldsField.First(f => f.Name.Equals(fieldName));
            if (result == null) return null;
            EnsureTypeBuilder();
            return result.FieldBuilder;
        }

        public DynamicMethodInfo WithMethod(string methodName)
        {
            return new DynamicMethodInfo(this, methodName);
        }

        public DynamicTypeInfo WithProperty(
            string propertyName,
            Type propertyType,
            Action<DynamicMethodBody> getmethod,
            Action<DynamicMethodBody> setmethod = null
            )
        {
            PropertyBuilder property = TypeBuilder.DefineProperty(
                propertyName,
                PropertyAttributes.None,
                propertyType,
                new Type[] {}
                );

            DynamicMethodInfo getMethodinfo = WithMethod(string.Format("get_{0}", propertyName))
                .TurnOnAttributes(MethodAttributes.RTSpecialName)
                .TurnOnAttributes(MethodAttributes.SpecialName);

            getmethod(getMethodinfo.Returns(propertyType));
            property.SetGetMethod(getMethodinfo.MethodBuilder);

            if (setmethod != null)
            {
                DynamicMethodInfo setMethodinfo = WithMethod(string.Format("set_{0}", propertyName))
                    .TurnOnAttributes(MethodAttributes.RTSpecialName)
                    .TurnOnAttributes(MethodAttributes.SpecialName)
                    .WithParameter(propertyType, "value");

                setmethod(setMethodinfo.Returns(typeof (void)));
                property.SetSetMethod(setMethodinfo.MethodBuilder);
            }
            return this;
        }


        public DynamicTypeInfo WithAutoProperty(
            string propertyName,
            Type propertyType
            )
        {
            string fieldName = string.Format("_{0}", Guid.NewGuid());
            return WithField(fieldName, propertyType)
                .WithProperty(
                    propertyName,
                    propertyType,
                    mget => mget
                                  .Ldarg(0) // this;
                                  .Ldfld(fieldName)
                                  .Ret(),
                    mset => mset
                                  .Ldarg(0) // this;
                                  .Ldarg("value")
                                  .Stfld(fieldName)
                                  .Ret()
                );
        }
    }
}