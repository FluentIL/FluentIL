using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FluentIL.Infos
{
    public class DynamicMethodInfo
    {
        public DynamicMethodInfo(
            DynamicTypeInfo dynamicTypeInfo,
            string methodName
            )
            : this()
        {
#if DEBUG
            Debug.Print(".method {0}", methodName);
#endif

            DynamicTypeInfo = dynamicTypeInfo;
            MethodName = methodName;
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

        internal DynamicTypeInfo DynamicTypeInfo { get; private set; }

        public MethodBuilder MethodBuilder
        {
            get { return methodBuilderField; }
        }

        public ILGenerator GetILGenerator()
        {
            if (DynamicTypeInfo == null)
                return AsDynamicMethod.GetILGenerator();
            
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
            return methodBuilderField.GetILGenerator();
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

        public DynamicMethodInfo WithParameter(Type parameterType, string parameterName = "")
        {
#if DEBUG
            Debug.Print(".param ({0}) [{1}] {2}",
                        Parameters.Count() + (DynamicTypeInfo == null ? 0 : 1),
                        parameterType,
                        String.IsNullOrEmpty(parameterName) ? "no-name" : parameterName);
#endif
            parametersField.Add(new DynamicVariableInfo(parameterName, parameterType));
            return this;
        }

        public DynamicMethodInfo WithParameter<T>(string parameterName = "")
        {
#if DEBUG
            Debug.Print(".param ({0}) [{1}] {2}",
                        Parameters.Count() + (DynamicTypeInfo == null ? 0 : 1),
                        typeof(T),
                        String.IsNullOrEmpty(parameterName) ? "no-name" : parameterName);
#endif
            parametersField.Add(new DynamicVariableInfo(parameterName, typeof(T)));
            return this;            
        }



        public DynamicMethodInfo WithVariable(Type variableType, string variableName = "")
        {
#if DEBUG
            Debug.Print(".local ({0}) [{1}] {2}",
                        Variables.Count(),
                        variableType,
                        String.IsNullOrEmpty(variableName) ? "no-name" : variableName);
#endif
            variablesField.Add(new DynamicVariableInfo(variableName, variableType));
            return this;
        }

        public DynamicMethodBody Returns(Type type)
        {
            ReturnType = type;
#if DEBUG
            Debug.Print("returns {0}", type);
#endif

            return Body;
        }

        public DynamicMethodBody Returns<T>()
        {
            ReturnType = typeof (T);

#if DEBUG
            Debug.Print("returns {0}", ReturnType);
#endif

            return Body;
        }

        public DynamicMethodBody ReturnsInteger()
        {
            ReturnType = typeof (int);

#if DEBUG
            Debug.Print("returns {0}", ReturnType);
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
    }
}