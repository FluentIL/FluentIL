using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using FluentIL.Emitters;

namespace FluentIL.Infos
{
    /// <summary>
    /// since both DynamicMethodInfo and DynamicConstructorInfo share DynamicMethodBody as abstraction
    /// of IL in ctor/method bodies, both share common interface between them. 
    /// </summary>
    public interface IDynamicMethodInfo
    {
        Type Owner { get; }

        string MethodName { get; }

        DynamicMethod AsDynamicMethod { get; }

        ILEmitter GetILEmitter();

        IDynamicMethodInfo WithVariable(Type variableType, string variableName = "");

        IDynamicMethodInfo WithVariable<T>(string variableName = "");

        IEnumerable<DynamicVariableInfo> Parameters { get; }

        IEnumerable<DynamicVariableInfo> Variables { get; }

        DynamicTypeInfo DynamicTypeInfo { get; }

    }
}
