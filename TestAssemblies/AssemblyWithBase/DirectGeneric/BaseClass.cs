using System.ComponentModel;

namespace AssemblyWithBase.DirectGeneric;

public class BaseClass<T> : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
    public virtual void OnPropertyChanging(string propertyName)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }
}