using System.ComponentModel;

namespace AssemblyWithBase.BaseWithGenericParent;

public class BaseClass1<T> : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
    public virtual void OnPropertyChanging(string propertyName)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }
}