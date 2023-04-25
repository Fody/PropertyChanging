using System.ComponentModel;

public class ClassAlreadyHasNotification : INotifyPropertyChanging
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            OnPropertyChanging("Property1");
            OnPropertyChanging("Property2");
            property1 = value;
        }
    }

    public virtual void OnPropertyChanging(string propertyName)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }

    public event PropertyChangingEventHandler PropertyChanging;
}