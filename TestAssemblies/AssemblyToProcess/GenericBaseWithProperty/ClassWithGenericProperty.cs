using System.ComponentModel;

namespace GenericBaseWithProperty;

public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanging
{
    public T Property1 { get; set; }
    public event PropertyChangingEventHandler PropertyChanging;
}