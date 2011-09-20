using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace FluentIL
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Call(MethodInfo methodInfo)
        {
            return this.Emit(OpCodes.Call, methodInfo);
        }

        public DynamicMethodBody Call<T>(string methodName, params Type[] types)
        {
            MethodInfo mi;
            if (types.Length > 0)
                mi = typeof(T).GetMethod(methodName, types);
            else
                mi = typeof(T).GetMethod(methodName);
            
            return this.Call(mi);
        }

        public DynamicMethodBody CallGet<T>(string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            return this.CallGet(property);
        }


        public DynamicMethodBody CallGet(PropertyInfo property)
        {
            return this.Call(property.GetGetMethod());
        }

        public DynamicMethodBody CallSet<T>(string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            return this.Call(property.GetSetMethod());
        }

        public DynamicMethodBody CallSet<T>(object o, bool popsObject = true)
        {
            foreach (var p in o.GetType().GetProperties())
            {
                Dup();
                if (p.PropertyType == typeof(string))
                    Ldstr((string)p.GetValue(o, null));
                else if (p.PropertyType == typeof(double))
                    Ldc((double)p.GetValue(o, null));
                else if (p.PropertyType == typeof(int))
                    Ldc((int)p.GetValue(o, null));
                else if (p.PropertyType == typeof(float))
                    Ldc((float)p.GetValue(o, null));
                else
                    throw new NotSupportedException();
                CallSet<T>(p.Name);
            }

            if (popsObject) Pop();
            return this;
        }

    }
}
