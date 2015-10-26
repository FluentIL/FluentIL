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
        private readonly List<DynamicFieldInfo> _fields = new List<DynamicFieldInfo>();
        private readonly List<Type> _interfaces = new List<Type>();
        private Type _parent = typeof(object);
        private TypeBuilder _typeBuilder;

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
                return _typeBuilder;
            }
        }

        public Type AsType => Complete();

        private void EnsureTypeBuilder()
        {
            if (_typeBuilder != null) return;
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

            _typeBuilder = _moduleBuilder.DefineType(TypeName,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                _parent,
                _interfaces.ToArray()
                );
                
            foreach (var field in _fields)
            {
                field.FieldBuilder = _typeBuilder.DefineField(
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
            _interfaces.Add(typeof (TInterface));
#if DEBUG
            Console.WriteLine("implements {0}", typeof (TInterface));
#endif
            return this;
        }

        public DynamicTypeInfo Inherits<TBaseClass>()
            where TBaseClass : class
        {
            _parent = typeof(TBaseClass);
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

            _fields.Add(
                value
                );

            if (_typeBuilder != null)
            {
                value.FieldBuilder = _typeBuilder.DefineField(
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

        public DynamicMethodInfo WithStaticMethod(string methodName)
        {
            return new DynamicMethodInfo(this, methodName)
                .TurnOnAttributes(MethodAttributes.HideBySig | MethodAttributes.Static)
                .TurnOffAttributes(MethodAttributes.Virtual);
        }
        
        public DynamicTypeInfo WithMethod(string methodName, Action<DynamicMethodInfo> methodDefinition)
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

        public Type Complete()
        {
            return TypeBuilder.CreateType();
        }
    }
}