using System.ComponentModel;

namespace AssemblyWithBase.GenericFromAbove;

public class BaseClass2 :
    BaseClass1<object>;

public class BaseClass3 :
    BaseClass2,
    INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
    public virtual void OnPropertyChanging(string propertyName)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }
}