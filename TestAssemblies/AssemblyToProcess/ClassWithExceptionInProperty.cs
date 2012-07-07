using System;
using System.ComponentModel;

public class ClassWithExceptionInProperty : INotifyPropertyChanging
{
    public string Property
    {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}