using FluentIL.Emitters;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FluentIL.Infos
{
    public class DynamicPropertyInfo
    {
        public DynamicPropertyInfo(
            DynamicTypeInfo dti,
            string name,
            Type type)
        {
            Owner = dti;
            Name = name;
            Type = type;
        }

        private void EnsurePropertyBuilder()
        {
            if (PropertyBuilder != null) return;

            PropertyBuilder = Owner.TypeBuilder.DefineProperty(
                                    RealName,
                                    PropertyAttributes.None,
                                    Type,
                                    Type.EmptyTypes                                    
                                    );
        }

        public DynamicMethodBody WithGetter()
        {
            EnsurePropertyBuilder();

            var getMethodinfo = Owner
                .WithMethod($"get_{Name}")
                .TurnOnAttributes(MethodAttributes.RTSpecialName)
                .TurnOnAttributes(MethodAttributes.SpecialName);

            getMethodinfo.Returns(Type);
            getMethodinfo.GetILEmitter();
            PropertyBuilder.SetGetMethod(getMethodinfo.MethodBuilder);

            return getMethodinfo.Body;
        }

        public DynamicMethodBody WithSetter()
        {
            EnsurePropertyBuilder();

            var setMethodInfo = Owner
                .WithMethod($"set_{Name}")
                .TurnOnAttributes(MethodAttributes.RTSpecialName)
                .TurnOnAttributes(MethodAttributes.SpecialName)
                .WithParameter(Type, "value");

            setMethodInfo.Returns(typeof(void));
            setMethodInfo.GetILEmitter();
            PropertyBuilder.SetSetMethod(setMethodInfo.MethodBuilder);

            return setMethodInfo.Body;
        }

        public DynamicPropertyInfo ImplementsExplicitly<TInterface>()
        {
            return ImplementsExplicitly(typeof(TInterface));
        }

        public DynamicPropertyInfo ImplementsExplicitly(Type interfaceType)
        {
            ExplImplementedInterface = interfaceType;
            return this;
        }

        public Type ExplImplementedInterface { get; private set; }
        public string Name { get; private set; }
        public string RealName
        {
            get
            {
                if (ExplImplementedInterface != null)
                    return $"{ExplImplementedInterface.FullName}.{Name}";

                return Name;
            }
        }

        public DynamicTypeInfo Owner { get; private set; }
        public Type Type { get; private set; }
        public PropertyBuilder PropertyBuilder { get; internal set; }

        public void WithAutoGetterSetter()
        {
            var emiter = new PropertyEmitter(this);
            emiter.EmitAuto();
        }
    }
}