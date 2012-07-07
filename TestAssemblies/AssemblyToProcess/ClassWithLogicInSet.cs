﻿using System.ComponentModel;
using System.Diagnostics;

public class ClassWithLogicInSet : INotifyPropertyChanging
{

    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            Debug.WriteLine("Foo");
            property1 = value;
            Debug.WriteLine("Bar");
        }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}
