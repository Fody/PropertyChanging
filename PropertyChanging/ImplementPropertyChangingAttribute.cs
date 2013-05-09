﻿using System;

namespace PropertyChanging
{
    /// <summary>
    /// Include a <see cref="Type"/> for notification.
    /// The <see cref="System.ComponentModel.INotifyPropertyChanging"/> interface is added to the type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementPropertyChangingAttribute : Attribute
    {
    }
}