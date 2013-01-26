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
        private readonly List<Type> parametersField = new List<Type>();
        private readonly List<DynamicVariableInfo> variablesField = new List<DynamicVariableInfo>();
        private ConstructorBuilder constructorBuilderField;
        private ReflectionILEmitter reflectionILEmitterField;
        private DynamicMethodBody Body;
        private DynamicMethod ctorDynamicMethodField;
        internal DynamicTypeInfo DynamicTypeInfo { get; private set; }

        public DynamicConstructorInfo(DynamicTypeInfo dynamicTypeInfo, params Type[] constructorArgumentTypes)
        {
            DynamicTypeInfo = dynamicTypeInfo;            
#if DEBUG
            Console.WriteLine("{0}::.ctor",dynamicTypeInfo.TypeName);
#endif
            parametersField.AddRange(constructorArgumentTypes);
            InitConstructorBuilder(constructorArgumentTypes);
        }

        public Type Owner
        {
            get { return constructorBuilderField.ReflectedType; }
        }

        public string MethodName
        {
            get { return ".ctor"; }
        }

        public DynamicMethod AsDynamicMethod
        {
            get { return GetCtorDynamicMethod(); }
        }

        public IEnumerable<DynamicVariableInfo> Parameters
        {
            get { return parametersField.Select(type => new DynamicVariableInfo(String.Empty, type)); }
        }

        public IEnumerable<DynamicVariableInfo> Variables
        {
            get { return variablesField; }
        }

        DynamicTypeInfo IDynamicMethodInfo.DynamicTypeInfo
        {
            get { return DynamicTypeInfo; }
        }
        
        public ILEmitter GetILEmitter()
        {
            return reflectionILEmitterField;
        }

        /// <summary>
        /// start definition of constructor method body with calling to 'Object' default constructor
        /// </summary>        
        public DynamicMethodBody BodyDefinition()
        {
            return Body;
        }

        /// <summary>
        /// start definition of constructor method body with calling to base class default constructor
        /// </summary>        
        public DynamicMethodBody BodyDefinitionWithDefaultBaseCtor()
        {
            Body.Ldarg(0)
                .Call(BaseConstructor());
            return Body;
        }

        /// <summary>
        /// retrieves public constructor based on parameter types
        /// </summary>
        /// <exception cref="ApplicationException">if constructor on base type is not found</exception>
        public ConstructorInfo BaseConstructor(params Type[] parameterTypes)
        {
            var retrievedConstructor = DynamicTypeInfo.TypeBuilder.BaseType.GetConstructor(parameterTypes);

            if (retrievedConstructor == null)
            {
                throw new ApplicationException("constructor on base type with specified parameter types not found");
            }

            return retrievedConstructor;
        }

        public DynamicConstructorInfo WithVariable(Type variableType, string variableName)
        {
            variablesField.Add(new DynamicVariableInfo(variableName, variableType));
            reflectionILEmitterField.DeclareLocal(variableType);
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
            constructorBuilderField = DynamicTypeInfo.TypeBuilder
                            .DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorArgumentTypes);

            reflectionILEmitterField = new ReflectionILEmitter(constructorBuilderField.GetILGenerator());
            Body = new DynamicMethodBody(this);
        }

        private DynamicMethod GetCtorDynamicMethod()
        {
            if (ctorDynamicMethodField == null)
            {
                ctorDynamicMethodField = new DynamicMethod(String.Empty,
                                                           constructorBuilderField.DeclaringType,
                                                           parametersField.ToArray());

                var ilgen = ctorDynamicMethodField.GetILGenerator();
                foreach (var variableToDeclare in Variables)
                    ilgen.DeclareLocal(variableToDeclare.Type);
            }
            return ctorDynamicMethodField;
        }

        #endregion
    }
}
