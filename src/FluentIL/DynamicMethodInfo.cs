using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Diagnostics;

namespace FluentIL
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

            this.DynamicTypeInfo = dynamicTypeInfo;
            this.MethodName = methodName;
        }

        //public DynamicMethodInfo(
        //    DynamicTypeInfo dynamicTypeInfo,
        //    MethodInfo methodInfo)
        //    : this(dynamicTypeInfo, methodInfo.Name)
        //{
            
        //    var attributes = methodInfo.Attributes;
        //    attributes -= MethodAttributes.Abstract;

        //    this.methodBuilder = dynamicTypeInfo.TypeBuilder.DefineMethod(methodInfo.Name,
        //        attributes, methodInfo.ReturnType, new Type[] { });
        //}

        public DynamicMethodInfo()
        {
            this.Body = new DynamicMethodBody(this);
            this.MethodName = "DynMethod";
        }

        
        public Type Owner { get; private set; }
        public string MethodName { get; private set; }

        #region DynamicMethod Gen

        DynamicMethod _result;
        public DynamicMethod AsDynamicMethod
        {
            get
            {
                if (this.DynamicTypeInfo != null)
                    throw new InvalidOperationException();

                if (_result == null)
                {
                    var parameterTypes = _Parameters.Select(p => p.Type)
                    .ToArray();

                    if (Owner != null)
                    {
                        _result = new DynamicMethod(
                            this.MethodName,
                            ReturnType,
                            parameterTypes,
                            Owner,
                            true
                            );
                    }
                    else
                    {
                        _result = new DynamicMethod(
                            this.MethodName,
                            ReturnType,
                            parameterTypes
                            );
                    }

                    var ilgen = _result.GetILGenerator();
                    foreach (var variable in this.Variables)
                        ilgen.DeclareLocal(variable.Type);
                }
                return _result;
            }
        }

        internal DynamicTypeInfo DynamicTypeInfo { get; private set; }

        MethodBuilder methodBuilder = null;

        public MethodBuilder MethodBuilder { get { return this.methodBuilder; } }

        public ILGenerator GetILGenerator()
        {
            if (this.DynamicTypeInfo == null)
                return this.AsDynamicMethod.GetILGenerator();
            else
            {
                if (this.methodBuilder == null)
                {
                    var parameterTypes = _Parameters.Select(p => p.Type)
                        .ToArray();

                    methodBuilder = this.DynamicTypeInfo.TypeBuilder.DefineMethod(
                        this.MethodName,
                        this.MethodAttributes,
                        CallingConventions.HasThis,
                        ReturnType,
                        parameterTypes);
                    
                    var ilgen = methodBuilder.GetILGenerator();
                    foreach (var variable in this.Variables)
                        ilgen.DeclareLocal(variable.Type);
                }
                return this.methodBuilder.GetILGenerator();
            }
        }

        #endregion

        #region DSL
        MethodAttributes MethodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;
        public DynamicMethodInfo TurnOnAttributes(MethodAttributes attributes)
        {
            this.MethodAttributes |= attributes;
            return this;
        }

        public DynamicMethodInfo TurnOffAttributes(MethodAttributes attributes)
        {
            this.MethodAttributes &= ~attributes;
            return this;
        }

        public DynamicMethodInfo WithOwner(Type owner)
        {
            this.Owner = owner;
            return this;
        }

        public DynamicMethodInfo WithParameter(Type parameterType, string parameterName = "")
        {
#if DEBUG
            Debug.Print(".param ({0}) [{1}] {2}", 
                this.Parameters.Count() + (this.DynamicTypeInfo == null ? 0 : 1),
                parameterType, 
                String.IsNullOrEmpty(parameterName) ? "no-name" : parameterName);
#endif
            this._Parameters.Add(new DynamicVariableInfo(parameterName, parameterType));
            return this;
        }

        public DynamicMethodInfo WithVariable(Type variableType, string variableName = "")
        {
#if DEBUG
            Debug.Print(".local ({0}) [{1}] {2}",
                this.Variables.Count() ,
                variableType,
                String.IsNullOrEmpty(variableName) ? "no-name" : variableName);
#endif
            this._Variables.Add(new DynamicVariableInfo(variableName, variableType));
            return this;
        }

        public DynamicMethodBody Returns(Type type)
        {
            this.ReturnType = type;
#if DEBUG
            Debug.Print("returns {0}", type);
#endif

            return this.Body;
        }

        public object Invoke(params object[] args)
        {
            return this.AsDynamicMethod.Invoke(null, args);
        }
        #endregion

        #region Properties

        public Type ReturnType { get; private set; }

        public DynamicMethodBody Body { get; private set; }

        readonly List<DynamicVariableInfo> _Parameters = new List<DynamicVariableInfo>();
        public IEnumerable<DynamicVariableInfo> Parameters
        {
            get
            {
                return _Parameters;
            }
        }


        readonly List<DynamicVariableInfo> _Variables = new List<DynamicVariableInfo>();
        public IEnumerable<DynamicVariableInfo> Variables
        {
            get
            {
                return _Variables;
            }
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
