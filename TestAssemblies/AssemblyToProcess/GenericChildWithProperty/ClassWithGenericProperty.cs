using System.ComponentModel;

namespace GenericChildWithProperty;

public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;

}