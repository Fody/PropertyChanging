using System;

namespace PropertyChanging
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DoNotNotifyAttribute : Attribute
    {
    }
}