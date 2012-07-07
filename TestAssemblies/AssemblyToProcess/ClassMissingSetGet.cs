using System.ComponentModel;

public class ClassMissingSetGet : INotifyPropertyChanging
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

}