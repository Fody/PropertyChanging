using System.ComponentModel;

public class ClassAlreadyHasSingleNotificationDiffSignature : INotifyPropertyChanging
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            OnPropertyChanging("Property1", 9);
            property1 = value;
        }
    }

    public virtual void OnPropertyChanging(string propertyName,int fake)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }

    public event PropertyChangingEventHandler PropertyChanging;
}