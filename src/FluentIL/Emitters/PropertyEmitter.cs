using System;
using System.Reflection;
using FluentIL.Infos;

namespace FluentIL.Emitters
{
    internal class PropertyEmitter
    {
        private readonly DynamicPropertyInfo _dynamicPropertyInfo;

        public PropertyEmitter(DynamicPropertyInfo dynamicPropertyInfo)
        {
            _dynamicPropertyInfo = dynamicPropertyInfo;
        }

        public void EmitAuto()
        {
            var fieldName = $"_{Guid.NewGuid()}";
            _dynamicPropertyInfo
                .Owner
                .WithField(fieldName, _dynamicPropertyInfo.Type);

            _dynamicPropertyInfo
                .WithGetter()
                    .Ldarg(0) // this;
                    .Ldfld(fieldName)
                    .Ret();

            _dynamicPropertyInfo
                .WithSetter()
                    .Ldarg(0) // this;
                    .Ldarg("value")
                    .Stfld(fieldName)
                    .Ret();
        }
    }
}