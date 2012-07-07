using System.ComponentModel;

public class ClassWithOwnImplementation : INotifyPropertyChanging
{
    public string Property1 { get; set; }
    bool baseNotifyCalled;
    public bool BaseNotifyCalled
    {
        get { return baseNotifyCalled; }
    }

    public event PropertyChangingEventHandler PropertyChanging;

    public virtual void OnPropertyChanging(string propertyName)
    {
        baseNotifyCalled = true;
        var handler = PropertyChanging;
        if (handler != null)
        {
            handler(this, new PropertyChangingEventArgs(propertyName));
        }
    }

}