using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using FluentIL.Emitters;

namespace FluentIL.Infos
{
    public class DynamicTypeInfo
    {
        private ModuleBuilder _moduleBuilder;
        private readonly List<DynamicFieldInfo> _fieldsField = new List<DynamicFieldInfo>();
        private readonly List<Type> _interfacesField = new List<Type>();
        private Type _parentField = typeof(object);
        private TypeBuilder _typeBuilderField;

        public DynamicTypeInfo(string typeName)
        {
            TypeName = typeName;
#if DEBUG
            Console.WriteLine(".class {0}", TypeName);
#endif
        }

        internal DynamicTypeInfo(string typeName, ModuleBuilder moduleBuilder)
            : this(typeName)
        {
            _moduleBuilder = moduleBuilder;
        }

        public string TypeName { get; private set; }

        public TypeBuilder TypeBuilder
        {
            get
            {
                EnsureTypeBuilder();
                return _typeBuilderField;
            }
        }

        public Type AsType => TypeBuilder.CreateType();

        private void EnsureTypeBuilder()
        {
            if (_typeBuilderField != null) return;
            if (_moduleBuilder == null)
            {
                var assemblyName = new AssemblyName(
                    $"__assembly__{DateTime.Now.Millisecond}"
                    );

                var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                    assemblyName,
                    AssemblyBuilderAccess.RunAndSave
                    );

                _moduleBuilder = assemblyBuilder.DefineDynamicModule(
                    assemblyBuilder.GetName().Name,
                    false
                    );
            }

            _typeBuilderField = _moduleBuilder.DefineType(TypeName,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                _parentField,
                _interfacesField.ToArray()
                );
                
            foreach (var field in _fieldsField)
            {
                field.FieldBuilder = _typeBuilderField.DefineField(
                    field.Name,
                    field.Type,
                    FieldAttributes.Private
                    );
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
            _interfacesField.Add(typeof (TInterface));
#if DEBUG
            Console.WriteLine("implements {0}", typeof (TInterface));
#endif
            return this;
        }

        public DynamicTypeInfo Inherits<TBaseClass>()
            where TBaseClass : class
        {
            _parentField = typeof(TBaseClass);
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

            _fieldsField.Add(
                value
                );

            if (_typeBuilderField != null)
            {
                value.FieldBuilder = _typeBuilderField.DefineField(
                    fieldName,
                    fieldType,
                    FieldAttributes.Private
                    );
            }
            return this;
        }

        public FieldInfo GetFieldInfo(string fieldName)
        {
            var result = _fieldsField.First(f => f.Name.Equals(fieldName));
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