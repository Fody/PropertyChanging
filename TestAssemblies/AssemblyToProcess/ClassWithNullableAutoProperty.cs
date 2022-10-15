using System.ComponentModel;

public class ClassWithNullableAutoProperty: INotifyPropertyChanging
{
    public bool? IsFlag
    {
        get;
        set;
    }

    public event PropertyChangingEventHandler PropertyChanging;
}
