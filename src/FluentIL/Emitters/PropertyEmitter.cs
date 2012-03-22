using System;
using System.Reflection;
using System.Reflection.Emit;
using FluentIL.Infos;

namespace FluentIL.Emitters
{
    internal class PropertyEmitter
    {
        private readonly DynamicTypeInfo dynamicTypeInfoField;

        public PropertyEmitter(DynamicTypeInfo dynamicTypeInfo)
        {
            dynamicTypeInfoField = dynamicTypeInfo;
        }

        public void Emit(
            string propertyName,
            Type propertyType,
            Action<DynamicMethodBody> getmethod,
            Action<DynamicMethodBody> setmethod = null
            )
        {
            PropertyBuilder property = dynamicTypeInfoField.TypeBuilder.DefineProperty(
                propertyName,
                PropertyAttributes.None,
                propertyType,
                new Type[] {}
                );

            DynamicMethodInfo getMethodinfo = dynamicTypeInfoField
                .WithMethod(string.Format("get_{0}", propertyName))
                .TurnOnAttributes(MethodAttributes.RTSpecialName)
                .TurnOnAttributes(MethodAttributes.SpecialName);

            getmethod(getMethodinfo.Returns(propertyType));
            property.SetGetMethod(getMethodinfo.MethodBuilder);

            if (setmethod != null)
            {
                DynamicMethodInfo setMethodinfo = dynamicTypeInfoField
                    .WithMethod(string.Format("set_{0}", propertyName))
                    .TurnOnAttributes(MethodAttributes.RTSpecialName)
                    .TurnOnAttributes(MethodAttributes.SpecialName)
                    .WithParameter(propertyType, "value");

                setmethod(setMethodinfo.Returns(typeof (void)));
                property.SetSetMethod(setMethodinfo.MethodBuilder);
            }
        }


        public void Emit(
            string propertyName,
            Type propertyType
            )
        {
            string fieldName = string.Format("_{0}", Guid.NewGuid());
            dynamicTypeInfoField
                .WithField(fieldName, propertyType)
                .WithProperty(
                    propertyName,
                    propertyType,
                    mget => mget
                                .Ldarg(0) // this;
                                .Ldfld(fieldName)
                                .Ret(),
                    mset => mset
                                .Ldarg(0) // this;
                                .Ldarg("value")
                                .Stfld(fieldName)
                                .Ret()
                );
        }
    }
}