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
        private Type parentField = typeof(object);
        private TypeBuilder typeBuilderField;

        public DynamicTypeInfo(string typeName)
        {
            TypeName = typeName;
#if DEBUG
            Console.WriteLine(".class {0}", TypeName);
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
                                                            parentField,
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

        public DynamicTypeInfo Repeater<T>(IEnumerable<T> itemCollection, Action<T, DynamicTypeInfo> actionMethod)
        {
            foreach (var item in itemCollection)
            {
                actionMethod(item,this);
            }

            return this;
        }

        public DynamicTypeInfo Implements<TInterface>()
        {
            interfacesField.Add(typeof (TInterface));
#if DEBUG
            Console.WriteLine("implements {0}", typeof (TInterface));
#endif
            return this;
        }

        public DynamicTypeInfo Inherits<TBaseClass>()
            where TBaseClass : class
        {
            parentField = typeof(TBaseClass);
#if DEBUG
            Console.WriteLine("inherits {0}", typeof(TBaseClass));
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
            Console.WriteLine(".field ({0}) {1}", fieldType, fieldName);
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

        public DynamicTypeInfo WithConstructor(Action<DynamicConstructorInfo> constructorDefinition, params Type[] argumentTypes)
        {
            var newConstructorInfo = new DynamicConstructorInfo(this, argumentTypes);
            constructorDefinition(newConstructorInfo);
            return this;
        }

        public DynamicMethodInfo WithMethod(string methodName)
        {
            return new DynamicMethodInfo(this, methodName);
        }
        
        public DynamicTypeInfo WithMethod(string methodName,Action<DynamicMethodInfo> methodDefinition)
        {
            var newMethodInfo = new DynamicMethodInfo(this, methodName);
            methodDefinition(newMethodInfo);
            
            return this;
        }        

        public DynamicTypeInfo WithProperty(
            string propertyName,
            Type propertyType,
            Action<DynamicMethodBody> getmethod,
            Action<DynamicMethodBody> setmethod = null
            )
        {
            new PropertyEmitter(this)
                .Emit(propertyName, propertyType, getmethod, setmethod);

            return this;
        }


        public DynamicTypeInfo WithAutoProperty(
            string propertyName,
            Type propertyType
            )
        {
            new PropertyEmitter(this)
                .Emit(propertyName, propertyType);
            
            return this;
        }
    }
}