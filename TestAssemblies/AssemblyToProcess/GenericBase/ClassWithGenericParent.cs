using System.ComponentModel;

public class ClassWithGenericParent<T> : INotifyPropertyChanging
{
    public string Property1 { get; set; }
    public event PropertyChangingEventHandler PropertyChanging;
}