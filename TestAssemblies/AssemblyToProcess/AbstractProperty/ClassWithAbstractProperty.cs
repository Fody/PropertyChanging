using System.ComponentModel;

public abstract class ClassWithAbstractProperty : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;

    public abstract string Property1 { get; set; }
}