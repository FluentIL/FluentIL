using System;
using System.Collections.Generic;
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

        private readonly ILEmitter _emitter;
        public DynamicMethodInfo(ILEmitter emitter)
        {
            Body = new DynamicMethodBody(this);
            _emitter = emitter;
        }

        public DynamicMethodInfo()
        {
            Body = new DynamicMethodBody(this);
            MethodName = "DynMethod";
        }


        public Type Owner { get; private set; }
        public string MethodName { get; private set; }

        #region DynamicMethod Gen

        private DynamicMethod _result;

        public DynamicMethod AsDynamicMethod
        {
            get
            {
                if (DynamicTypeInfo != null)
                    throw new InvalidOperationException();

                if (_result != null) return _result;
                var parameterTypes = _parametersField.Select(p => p.Type)
                    .ToArray();

                if (Owner != null)
                {
                    _result = new DynamicMethod(
                        MethodName,
                        ReturnType,
                        parameterTypes,
                        Owner,
                        true
                        );
                }
                else
                {
                    _result = new DynamicMethod(
                        MethodName,
                        ReturnType,
                        parameterTypes
                        );
                }

                var ilgen = _result.GetILGenerator();
                foreach (var variable in Variables)
                    ilgen.DeclareLocal(variable.Type);
                return _result;
            }
        }

        public DynamicTypeInfo DynamicTypeInfo { get; }

        public MethodBuilder MethodBuilder { get; private set; }

        public ILEmitter GetILEmitter()
        {
            if (_emitter != null)
                return _emitter;

            if (DynamicTypeInfo == null)
                return new ReflectionILEmitter(AsDynamicMethod.GetILGenerator());

            if (MethodBuilder != null) return new ReflectionILEmitter(MethodBuilder.GetILGenerator());
            var parameterTypes = _parametersField.Select(p => p.Type)
                .ToArray();

            if ((_methodAttributes & MethodAttributes.Static) == MethodAttributes.Static)
            {
                MethodBuilder = DynamicTypeInfo.TypeBuilder.DefineMethod(
                    MethodName,
                    _methodAttributes,
                    ReturnType,
                    parameterTypes);
            }
            else
            {
                MethodBuilder = DynamicTypeInfo.TypeBuilder.DefineMethod(
                    MethodName,
                    _methodAttributes,
                    CallingConventions.HasThis,
                    ReturnType,
                    parameterTypes);
            }
            

            var ilgen = MethodBuilder.GetILGenerator();
            foreach (var variable in Variables)
                ilgen.DeclareLocal(variable.Type);
            return new ReflectionILEmitter(MethodBuilder.GetILGenerator());
        }

        #endregion

        #region DSL

        private MethodAttributes _methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;

        public DynamicMethodInfo TurnOnAttributes(MethodAttributes attributes)
        {
            _methodAttributes |= attributes;
            return this;
        }

        public DynamicMethodInfo TurnOffAttributes(MethodAttributes attributes)
        {
            _methodAttributes &= ~attributes;
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
                        string.IsNullOrEmpty(parameterName) ? "no-name" : parameterName);
#endif
            _parametersField.Add(new DynamicVariableInfo(parameterName, parameterType));
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
                        string.IsNullOrEmpty(variableName) ? "no-name" : variableName);
#endif
            _variablesField.Add(new DynamicVariableInfo(variableName, variableType));
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

        private readonly List<DynamicVariableInfo> _parametersField = new List<DynamicVariableInfo>();
        private readonly List<DynamicVariableInfo> _variablesField = new List<DynamicVariableInfo>();
        public Type ReturnType { get; private set; }

        public DynamicMethodBody Body { get; }

        public IEnumerable<DynamicVariableInfo> Parameters => _parametersField;


        public IEnumerable<DynamicVariableInfo> Variables => _variablesField;

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