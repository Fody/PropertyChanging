using System.ComponentModel;

public class ClassNoBackingNoEqualityField : INotifyPropertyChanging
{

    public string StringProperty
    {
        get { return "Foo"; }
        set { }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}