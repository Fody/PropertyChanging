﻿using System.ComponentModel;
using PropertyChanging;

public class ClassWithBeforeImplementation : INotifyPropertyChanging
{
    public string Property1 { get; set; }

    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;

    public void OnPropertyChanging(string propertyName, object before)
    {
        ValidateIsString(before);

        PropertyChanging?.Invoke(this, new(propertyName));
    }

    static void ValidateIsString(object value)
    {
        if (value == null)
        {
            return;
        }

        var name = value.GetType().Name;
        if (name != "String")
        {
            throw new($"Value should be string but is '{name}'.");
        }
    }
}