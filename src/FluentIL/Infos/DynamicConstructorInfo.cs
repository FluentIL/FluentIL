using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using FluentIL.Emitters;

namespace FluentIL.Infos
{
    public class DynamicConstructorInfo : IDynamicMethodInfo
    {
        private readonly List<Type> _parametersField = new List<Type>();
        private readonly List<DynamicVariableInfo> _variablesField = new List<DynamicVariableInfo>();
        private ConstructorBuilder _constructorBuilderField;
        private ReflectionILEmitter _reflectionILEmitterField;
        private DynamicMethodBody _body;
        private DynamicMethod _ctorDynamicMethodField;
        internal DynamicTypeInfo DynamicTypeInfo { get; }

        public DynamicConstructorInfo(DynamicTypeInfo dynamicTypeInfo, params Type[] constructorArgumentTypes)
        {
            DynamicTypeInfo = dynamicTypeInfo;            
#if DEBUG
            Console.WriteLine("{0}::.ctor",dynamicTypeInfo.TypeName);
#endif
            _parametersField.AddRange(constructorArgumentTypes);
            InitConstructorBuilder(constructorArgumentTypes);
        }

        public Type Owner => _constructorBuilderField.ReflectedType;

        public string MethodName => ".ctor";

        public DynamicMethod AsDynamicMethod => GetCtorDynamicMethod();

        public IEnumerable<DynamicVariableInfo> Parameters
        {
            get { return _parametersField.Select(type => new DynamicVariableInfo(string.Empty, type)); }
        }

        public IEnumerable<DynamicVariableInfo> Variables => _variablesField;

        DynamicTypeInfo IDynamicMethodInfo.DynamicTypeInfo => DynamicTypeInfo;

        public ILEmitter GetILEmitter()
        {
            return _reflectionILEmitterField;
        }

        /// <summary>
        /// start definition of constructor method body with calling to 'Object' default constructor
        /// </summary>        
        public DynamicMethodBody BodyDefinition()
        {
            return _body;
        }

        /// <summary>
        /// start definition of constructor method body with calling to base class default constructor
        /// </summary>        
        public DynamicMethodBody BodyDefinitionWithDefaultBaseCtor()
        {
            _body.Ldarg(0)
                .Call(BaseConstructor());
            return _body;
        }

        /// <summary>
        /// retrieves public constructor based on parameter types
        /// </summary>
        /// <exception cref="ApplicationException">if constructor on base type is not found</exception>
        public ConstructorInfo BaseConstructor(params Type[] parameterTypes)
        {
            var retrievedConstructor = DynamicTypeInfo?.TypeBuilder?.BaseType?.GetConstructor(parameterTypes);

            if (retrievedConstructor == null)
            {
                throw new ApplicationException("constructor on base type with specified parameter types not found");
            }

            return retrievedConstructor;
        }

        public DynamicConstructorInfo WithVariable(Type variableType, string variableName)
        {
            _variablesField.Add(new DynamicVariableInfo(variableName, variableType));
            _reflectionILEmitterField.DeclareLocal(variableType);
            return this;
        }

        public DynamicConstructorInfo WithVariable<T>(string variableName)
        {
            return WithVariable(typeof(T), variableName);
        }

        IDynamicMethodInfo IDynamicMethodInfo.WithVariable(Type variableType, string variableName)
        {
            return WithVariable(variableType, variableName);
        }

        IDynamicMethodInfo IDynamicMethodInfo.WithVariable<T>(string variableName)
        {
            return WithVariable<T>(variableName);
        }

        #region Helper Methods
        
        private void InitConstructorBuilder(Type[] constructorArgumentTypes)
        {
            _constructorBuilderField = DynamicTypeInfo.TypeBuilder
                            .DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorArgumentTypes);

            _reflectionILEmitterField = new ReflectionILEmitter(_constructorBuilderField.GetILGenerator());
            _body = new DynamicMethodBody(this);
        }

        private DynamicMethod GetCtorDynamicMethod()
        {
            if (_ctorDynamicMethodField != null) return _ctorDynamicMethodField;
            _ctorDynamicMethodField = new DynamicMethod(string.Empty,
                _constructorBuilderField.DeclaringType,
                _parametersField.ToArray());

            var ilgen = _ctorDynamicMethodField.GetILGenerator();
            foreach (var variableToDeclare in Variables)
                ilgen.DeclareLocal(variableToDeclare.Type);
            return _ctorDynamicMethodField;
        }

        #endregion
    }
}
