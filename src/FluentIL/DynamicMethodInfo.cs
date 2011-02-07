using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace FluentIL
{
    public class DynamicMethodInfo
    {
        public DynamicMethodInfo()
        {
            this.Body = new DynamicMethodBody(this);
        }

        #region DynamicMethod Gen
        
        DynamicMethod _result;
        public DynamicMethod AsDynamicMethod
        {
            get
            {
                if (_result == null)
                {
                    var parameterTypes = _Parameters.Select(p => p.Type)
                    .ToArray();
                    _result = new DynamicMethod("DynMethod", ReturnType, parameterTypes);

                    var ilgen = _result.GetILGenerator();
                    foreach (var variable in this.Variables)
                        ilgen.DeclareLocal(variable.Type);
                }
                return _result;
            }
        }

        #endregion

        #region DSL
        public DynamicMethodInfo WithParameter(Type parameterType, string parameterName = "")
        {
            this._Parameters.Add(new DynamicVariableInfo(parameterName, parameterType));
            return this;
        }

        public DynamicMethodInfo WithVariable(Type variableType, string variableName = "")
        {
            this._Variables.Add(new DynamicVariableInfo(variableName, variableType));
            return this;
        }

        public DynamicMethodBody Returns(Type type)
        {
            this.ReturnType = type;
            return this.Body;
        }

        public object Invoke(params object [] args)
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
