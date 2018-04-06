using System.ComponentModel;
using PropertyChanging;
// ReSharper disable UnusedVariable

public class Class1 : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
    public Class1()
    {
        var type = typeof(DependsOnAttribute);
    }
}