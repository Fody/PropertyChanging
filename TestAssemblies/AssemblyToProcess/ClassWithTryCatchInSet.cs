using System;
using System.ComponentModel;

public class ClassWithTryCatchInSet:INotifyPropertyChanging
{
    string property1;
    public string Property1
    {
        get { return property1; }
        set
        {
            try
            {
                property1 = value;
            }
            catch (ArgumentException)
            {
                // actually, 'call OnPropertyChanging' inserted here, it's wrong.
            }
        }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}