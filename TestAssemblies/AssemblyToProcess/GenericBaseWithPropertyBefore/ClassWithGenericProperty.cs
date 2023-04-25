using System.ComponentModel;

namespace GenericBaseWithPropertyBefore;

public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanging
{
    public T Property1 { get; set; }
    public event PropertyChangingEventHandler PropertyChanging;
    public void OnPropertyChanging(string propertyName, object before)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }
}