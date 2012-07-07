using System.ComponentModel;

public class ClassWithFieldGetButNoFieldSet : INotifyPropertyChanging
{
    string property;

    public string Property1
    {
        get { return property; }
        set { SetField(value); }
    }

    void SetField(string value)
    {
        property = value;
    }

    public event PropertyChangingEventHandler PropertyChanging;
}