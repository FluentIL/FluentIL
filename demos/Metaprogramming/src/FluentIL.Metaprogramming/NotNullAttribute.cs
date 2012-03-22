using System;

namespace FluentIL.Metaprogramming
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class NotNullAttribute : Attribute
    {
    }
}