using System;
using System.Reflection;
using FluentIL.Infos;

namespace FluentIL.Emitters
{
    internal class PropertyEmitter
    {
        private readonly DynamicTypeInfo _dynamicTypeInfoField;

        public PropertyEmitter(DynamicTypeInfo dynamicTypeInfo)
        {
            _dynamicTypeInfoField = dynamicTypeInfo;
        }

        public void Emit(
            string propertyName,
            Type propertyType,
            Action<DynamicMethodBody> getmethod,
            Action<DynamicMethodBody> setmethod = null
            )
        {
            var property = _dynamicTypeInfoField.TypeBuilder.DefineProperty(
                propertyName,
                PropertyAttributes.None,
                propertyType,
                new Type[] {}
                );

            var getMethodinfo = _dynamicTypeInfoField
                .WithMethod($"get_{propertyName}")
                .TurnOnAttributes(MethodAttributes.RTSpecialName)
                .TurnOnAttributes(MethodAttributes.SpecialName);

            getmethod(getMethodinfo.Returns(propertyType));
            property.SetGetMethod(getMethodinfo.MethodBuilder);

            if (setmethod == null) return;
            var setMethodinfo = _dynamicTypeInfoField
                .WithMethod($"set_{propertyName}")
                .TurnOnAttributes(MethodAttributes.RTSpecialName)
                .TurnOnAttributes(MethodAttributes.SpecialName)
                .WithParameter(propertyType, "value");

            setmethod(setMethodinfo.Returns(typeof (void)));
            property.SetSetMethod(setMethodinfo.MethodBuilder);
        }


        public void Emit(
            string propertyName,
            Type propertyType
            )
        {
            var fieldName = $"_{Guid.NewGuid()}";
            _dynamicTypeInfoField
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