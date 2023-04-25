using System;

namespace PropertyChanging;

/// <summary>
/// Include a <see cref="Type"/> for notification.
/// The INotifyPropertyChanging interface is added to the type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ImplementPropertyChangingAttribute : Attribute
{
}