using System.ComponentModel;

namespace GenericChildWithPropertyOnChanging;

public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
}