using System.ComponentModel;

public class ClassAlreadyHasNotifcation : INotifyPropertyChanging
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            if (property1 != value)
            {
                property1 = value;
                OnPropertyChanging("Property1");
            }
        }
    }

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