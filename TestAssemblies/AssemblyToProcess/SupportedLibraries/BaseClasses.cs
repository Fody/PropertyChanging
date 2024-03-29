﻿using System.ComponentModel;
using PropertyChanging;

namespace ReactiveUI;

public abstract class ReactiveObject : INotifyPropertyChanging
{
    [DoNotNotify]
    public bool BaseNotifyCalled { get; set; }
    public event PropertyChangingEventHandler PropertyChanging;
    public virtual void raisePropertyChanging(string propertyName)
    {
        BaseNotifyCalled = true;
        PropertyChanging?.Invoke(this, new(propertyName));
    }
}