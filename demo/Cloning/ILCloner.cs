using System;
using System.Collections.Generic;
using System.Linq;
using FluentIL;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cloning
{
    public static class ILCloner
    {
        static readonly Dictionary<Type, Delegate> ShallowCloners =
            new Dictionary<Type, Delegate>();

        private static bool IsAnonymous(this Type that)
        {
            if (!that.IsGenericType)
                return false;

            if ((that.Attributes & TypeAttributes.NotPublic) != TypeAttributes.NotPublic)
                return false;

            if (!that.Name.Contains("AnonymousType"))
                return false;

            if (!(that.Name.StartsWith("<>") || that.Name.StartsWith("VB$")))
                return false;

            return Attribute.IsDefined(that, typeof(CompilerGeneratedAttribute), false);
        }

        private static Func<T, T> GenerateAnonymousTypeCloner<T>()
        {
            var type = typeof (T);
            var fields = type.GetFields(BindingFlags.Instance |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Public 
                );

            var method = IL.NewMethod()
                .WithParameter(type, "source")
                .WithOwner(type)
                .Returns(type);

            foreach (var field in fields)
            {
                method
                    .Ldarg("source")
                    .Ldfld(field);
                
            }

            var ci = type.GetConstructors().FirstOrDefault();
            method
                .Newobj(ci)
                .Ret();

            return (Func<T, T>)method.AsDynamicMethod.CreateDelegate(typeof(Func<T, T>));
        }

        private static IEnumerable<FieldInfo> GetAllFields(this Type that)
        {
            var type = that;
            while (type != typeof(object) && type != null)
            {
                var fields = type.GetFields(BindingFlags.Instance |
                                            BindingFlags.NonPublic |
                                            BindingFlags.Public
                    );

                foreach (var field in fields)
                    yield return field;

                type = type.BaseType;
            }
        }

        private static Func<T, T> GenerateConventionalCloner<T>()
        {
            var type = typeof (T);

            var method = IL.NewMethod()
                .WithParameter(type, "source")
                .WithVariable(type, "result")
                .WithOwner(type)
                .Returns(type)

                .Newobj<T>()
                .Stloc("result");

            foreach (var field in type.GetAllFields())
            {
                method
                    .Ldloc("result")
                    .Ldarg("source")
                    .Ldfld(field)
                    .Stfld(field);
            }

            method
                .Ldloc("result")
                .Ret();

            return (Func<T, T>) method.AsDynamicMethod.CreateDelegate(typeof(Func<T, T>));
        }

        public static Func<T, T> GetShallowCloner<T>(this T that)
            where T : class
        {
            var type = typeof(T);
            if (!ShallowCloners.ContainsKey(type))
            {
                ShallowCloners.Add(
                    type,
                    type.IsAnonymous() ? GenerateAnonymousTypeCloner<T>() : GenerateConventionalCloner<T>()
                    );
            }

            return (Func<T, T>)ShallowCloners[type];
        }

        public static T ShallowClone<T>(this T that)
            where T : class
        {
            var cloner = that.GetShallowCloner();
            return cloner(that);
        }
    }
}
