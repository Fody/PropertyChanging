using System.ComponentModel;

public class ClassWithFieldFromOtherClass : INotifyPropertyChanging
{
    OtherClass otherClass;

    public ClassWithFieldFromOtherClass()
    {
        otherClass = new();
    }

    public string Property1
    {
        get { return otherClass.property1; }
        set { otherClass.property1 = value; }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}