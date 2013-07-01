using System.ComponentModel;

public class ClassAlreadyHasSingleNotification : INotifyPropertyChanging
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            OnPropertyChanging("Property1");
            property1 = value;
        }
    }

    public string Property2 { get { return Property1; } }

    public virtual void OnPropertyChanging(string propertyName)
    {
        var handler = PropertyChanging;
        if (handler != null)
        {
            handler(this, new PropertyChangingEventArgs(propertyName));
        }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}