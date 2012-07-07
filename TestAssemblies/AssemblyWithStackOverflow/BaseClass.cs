    using System.ComponentModel;

public class BaseClass : INotifyPropertyChanging
{
    public virtual string Property1 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;

    protected void OnPropertyChanging(string propertyName, object before)
    {
        var handler = PropertyChanging;
        if (handler != null)
        {
            handler(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}