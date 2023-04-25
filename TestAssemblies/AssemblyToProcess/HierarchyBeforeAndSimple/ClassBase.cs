using System.ComponentModel;

namespace HierarchyBeforeAndSimple;

public class ClassBase : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;

    public void OnPropertyChanging(string propertyName)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }
}