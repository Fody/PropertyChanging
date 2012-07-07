using System.ComponentModel;

public class ClassWithPublicField : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;

    public bool Property1;
}