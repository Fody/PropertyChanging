using System.ComponentModel;

public class ClassAlreadyHasSingleNotificationDiffParamLocation : INotifyPropertyChanging
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            OnPropertyChanging(9, "Property1");
            property1 = value;
        }
    }

    public virtual void OnPropertyChanging(int fake, string propertyName)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }

    public event PropertyChangingEventHandler PropertyChanging;
}