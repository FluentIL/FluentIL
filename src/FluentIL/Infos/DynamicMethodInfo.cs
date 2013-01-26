using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FluentIL.Emitters;

namespace FluentIL.Infos
{
    public class DynamicMethodInfo : IDynamicMethodInfo 
    {
        public DynamicMethodInfo(
            DynamicTypeInfo dynamicTypeInfo,
            string methodName
            )
            : this()
        {
#if DEBUG
            Console.WriteLine(".method {0}", methodName);
#endif

            DynamicTypeInfo = dynamicTypeInfo;
            MethodName = methodName;
        }

        private readonly ILEmitter emitterField;
        public DynamicMethodInfo(ILEmitter emitter)
        {
            Body = new DynamicMethodBody(this);
            emitterField = emitter;
        }

        public DynamicMethodInfo()
        {
            Body = new DynamicMethodBody(this);
            MethodName = "DynMethod";
        }


        public Type Owner { get; private set; }
        public string MethodName { get; private set; }

        #region DynamicMethod Gen

        private DynamicMethod resultField;
        private MethodBuilder methodBuilderField;

        public DynamicMethod AsDynamicMethod
        {
            get
            {
                if (DynamicTypeInfo != null)
                    throw new InvalidOperationException();

                if (resultField == null)
                {
                    Type[] parameterTypes = parametersField.Select(p => p.Type)
                        .ToArray();

                    if (Owner != null)
                    {
                        resultField = new DynamicMethod(
                            MethodName,
                            ReturnType,
                            parameterTypes,
                            Owner,
                            true
                            );
                    }
                    else
                    {
                        resultField = new DynamicMethod(
                            MethodName,
                            ReturnType,
                            parameterTypes
                            );
                    }

                    ILGenerator ilgen = resultField.GetILGenerator();
                    foreach (DynamicVariableInfo variable in Variables)
                        ilgen.DeclareLocal(variable.Type);
                }
                return resultField;
            }
        }

        public DynamicTypeInfo DynamicTypeInfo { get; private set; }

        public MethodBuilder MethodBuilder
        {
            get { return methodBuilderField; }
        }

        public ILEmitter GetILEmitter()
        {
            if (emitterField != null)
                return emitterField;

            if (DynamicTypeInfo == null)
                return new ReflectionILEmitter(AsDynamicMethod.GetILGenerator());
            
            if (methodBuilderField == null)
            {
                Type[] parameterTypes = parametersField.Select(p => p.Type)
                    .ToArray();

                methodBuilderField = DynamicTypeInfo.TypeBuilder.DefineMethod(
                    MethodName,
                    methodAttributesField,
                    CallingConventions.HasThis,
                    ReturnType,
                    parameterTypes);

                ILGenerator ilgen = methodBuilderField.GetILGenerator();
                foreach (DynamicVariableInfo variable in Variables)
                    ilgen.DeclareLocal(variable.Type);
            }
            return new ReflectionILEmitter(methodBuilderField.GetILGenerator());
        }

        #endregion

        #region DSL

        private MethodAttributes methodAttributesField = MethodAttributes.Public | MethodAttributes.Virtual;

        public DynamicMethodInfo TurnOnAttributes(MethodAttributes attributes)
        {
            methodAttributesField |= attributes;
            return this;
        }

        public DynamicMethodInfo TurnOffAttributes(MethodAttributes attributes)
        {
            methodAttributesField &= ~attributes;
            return this;
        }

        public DynamicMethodInfo WithOwner(Type owner)
        {
            Owner = owner;
            return this;
        }

        public DynamicMethodInfo WithOwner<T>()
        {
            return WithOwner(typeof (T));
        }

        public DynamicMethodInfo WithParameters(IEnumerable<ParameterInfo> parameterCollection)
        {
            foreach (var parameter in parameterCollection)
            {
                WithParameter(parameter.ParameterType, parameter.Name);
            }
            
            return this;
        }

        public DynamicMethodInfo WithParameter(Type parameterType, string parameterName = "")
        {
#if DEBUG
            Console.WriteLine(".param ({0}) [{1}] {2}",
                        Parameters.Count() + (DynamicTypeInfo == null ? 0 : 1),
                        parameterType,
                        String.IsNullOrEmpty(parameterName) ? "no-name" : parameterName);
#endif
            parametersField.Add(new DynamicVariableInfo(parameterName, parameterType));
            return this;
        }

        public DynamicMethodInfo WithParameter<T>(string parameterName = "")
        {
            return WithParameter(typeof (T), parameterName);
        }

        public DynamicMethodInfo WithVariable(Type variableType, string variableName = "")
        {
#if DEBUG
            Console.WriteLine(".local ({0}) [{1}] {2}",
                        Variables.Count(),
                        variableType,
                        String.IsNullOrEmpty(variableName) ? "no-name" : variableName);
#endif
            variablesField.Add(new DynamicVariableInfo(variableName, variableType));
            return this;
        }

        public DynamicMethodInfo WithVariable<T>(string variableName = "")
        {
            return WithVariable(typeof (T), variableName);
        }

        public DynamicMethodBody Returns(Type type)
        {
            ReturnType = type;
#if DEBUG
            Console.WriteLine("returns {0}", type);
#endif

            return Body;
        }

        public DynamicMethodBody Returns<T>()
        {
            return Returns(typeof (T));
        }

        public DynamicMethodBody ReturnsInteger()
        {
            ReturnType = typeof (int);

#if DEBUG
            Console.WriteLine("returns {0}", ReturnType);
#endif

            return Body;
        }

        public object Invoke(params object[] args)
        {
            return AsDynamicMethod.Invoke(null, args);
        }

        #endregion

        #region Properties

        private readonly List<DynamicVariableInfo> parametersField = new List<DynamicVariableInfo>();
        private readonly List<DynamicVariableInfo> variablesField = new List<DynamicVariableInfo>();
        public Type ReturnType { get; private set; }

        public DynamicMethodBody Body { get; private set; }

        public IEnumerable<DynamicVariableInfo> Parameters
        {
            get { return parametersField; }
        }


        public IEnumerable<DynamicVariableInfo> Variables
        {
            get { return variablesField; }
        }

        #endregion

        #region static

        public static implicit operator DynamicMethod(DynamicMethodInfo info)
        {
            return info.AsDynamicMethod;
        }

        #endregion

        IDynamicMethodInfo IDynamicMethodInfo.WithVariable(Type variableType, string variableName)
        {
            return WithVariable(variableType, variableName);
        }

        IDynamicMethodInfo IDynamicMethodInfo.WithVariable<T>(string variableName)
        {
            return WithVariable<T>(variableName);
        }
    }
}