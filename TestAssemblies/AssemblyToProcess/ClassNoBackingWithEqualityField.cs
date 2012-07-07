using System.ComponentModel;

public class ClassNoBackingWithEqualityField : INotifyPropertyChanging
{

    public string StringProperty
    {
        get { return "Foo"; }
        set { }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}
