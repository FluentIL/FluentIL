using System;
using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Call(MethodInfo methodInfo)
        {
            return Emit(OpCodes.Call, methodInfo);
        }

        public DynamicMethodBody Callvirt(MethodInfo methodInfo)
        {
            return Emit(OpCodes.Callvirt, methodInfo);
        }

        public DynamicMethodBody Call<T>(string methodName, params Type[] types)
        {
            MethodInfo mi;
            if (types.Length > 0)
                mi = typeof (T).GetMethod(methodName, types);
            else
                mi = typeof (T).GetMethod(methodName);

            return Call(mi);
        }

        public DynamicMethodBody Callvirt<T>(string methodName, params Type[] types)
        {
            MethodInfo mi;
            if (types.Length > 0)
                mi = typeof(T).GetMethod(methodName, types);
            else
                mi = typeof(T).GetMethod(methodName);

            return Callvirt(mi);
        }

        
        public DynamicMethodBody CallGet<T>(string propertyName)
        {
            PropertyInfo property = typeof (T).GetProperty(propertyName);
            return CallGet(property);
        }


        public DynamicMethodBody CallGet(PropertyInfo property)
        {
            return Call(property.GetGetMethod());
        }

        public DynamicMethodBody CallSet<T>(string propertyName)
        {
            PropertyInfo property = typeof (T).GetProperty(propertyName);
            return Call(property.GetSetMethod());
        }

        public DynamicMethodBody CallSet<T>(object o, bool popsObject = true)
        {
            foreach (PropertyInfo p in o.GetType().GetProperties())
            {
                Dup();
                if (p.PropertyType == typeof (string))
                    Ldstr((string) p.GetValue(o, null));
                else if (p.PropertyType == typeof (double))
                    Ldc((double) p.GetValue(o, null));
                else if (p.PropertyType == typeof (int))
                    Ldc((int) p.GetValue(o, null));
                else if (p.PropertyType == typeof (float))
                    Ldc((float) p.GetValue(o, null));
                else
                    throw new NotSupportedException();
                CallSet<T>(p.Name);
            }

            if (popsObject) Pop();
            return this;
        }
    }
}