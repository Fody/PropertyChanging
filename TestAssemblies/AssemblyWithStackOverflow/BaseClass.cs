    using System.ComponentModel;

public class BaseClass : INotifyPropertyChanging
{
    public virtual string Property1 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;

    protected void OnPropertyChanging(string propertyName, object before)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }
}