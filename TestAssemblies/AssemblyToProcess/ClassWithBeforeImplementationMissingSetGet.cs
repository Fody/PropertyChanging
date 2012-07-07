using System.ComponentModel;

public class ClassWithBeforeImplementationMissingSetGet : INotifyPropertyChanging
{
    string property;

    public string PropertyNoSet
    {
        get { return property; }
    }


    public string PropertyNoGet
    {
        set { property = value; }
    }

    public event PropertyChangingEventHandler PropertyChanging;

    public void OnPropertyChanging(string propertyName, object before)
    {
        var handler = PropertyChanging;
        if (handler != null)
        {
            handler(this, new PropertyChangingEventArgs(propertyName));
        }
    }

}