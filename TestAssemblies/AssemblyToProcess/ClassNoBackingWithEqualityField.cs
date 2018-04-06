using System.ComponentModel;
// ReSharper disable ValueParameterNotUsed

public class ClassNoBackingWithEqualityField : INotifyPropertyChanging
{

    public string StringProperty
    {
        get { return "Foo"; }
        set { }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}
