using System;

namespace PropertyChanging
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute
    {
// ReSharper disable UnusedParameter.Local
        public DependsOnAttribute(string dependency, params string[] otherDependencies)
// ReSharper restore UnusedParameter.Local
        {

        }
    }
}