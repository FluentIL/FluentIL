using System;
using FluentIL.Infos;
using FluentIL.Emitters;

namespace FluentIL
{
    public class IL
    {
        public static DynamicMethodBody NewMethod
            (Type returnType, params Type[] parameterTypes)
        {
            var result = new DynamicMethodInfo();

            foreach (Type param in parameterTypes)
                result.WithParameter(param);

            result.Returns(returnType);

            return result.Body;
        }

        public static DynamicMethodBody EmitTo(ILEmitter emitter)
        {
            return new DynamicMethodInfo(emitter).Body;
        }

        public static DynamicMethodInfo NewMethod()
        {
            return new DynamicMethodInfo();
        }


        public static DynamicTypeInfo NewType()
        {
            return NewType("NewType" + Guid.NewGuid().ToString());
        }

        public static DynamicTypeInfo NewType(string typeName)
        {
            return new DynamicTypeInfo(typeName);
        }

        public static CatchBody<T> Catch<T>(Action<DynamicMethodBody> @body)
            where T : Exception
        {
            return new CatchBody<T>(@body);
        }
    }
}