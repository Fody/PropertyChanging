using System.ComponentModel;

public class ClassWithOwnImplementation : INotifyPropertyChanging
{
    public string Property1 { get; set; }
    bool baseNotifyCalled;
    public bool BaseNotifyCalled => baseNotifyCalled;

    public event PropertyChangingEventHandler PropertyChanging;

    public virtual void OnPropertyChanging(string propertyName)
    {
        baseNotifyCalled = true;
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }

}