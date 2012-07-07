using System.ComponentModel;

public class ClassParent : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
    public virtual void OnPropertyChanging(string propertyName)
    {
        var handler = PropertyChanging;
        if (handler != null)
        {
            handler(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}