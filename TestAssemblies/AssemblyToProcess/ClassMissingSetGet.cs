using System.ComponentModel;

public class ClassMissingSetGet : INotifyPropertyChanging
{
    string property;

    public string PropertyNoSet => property;


    public string PropertyNoGet
    {
        set { property = value; }
    }

    public event PropertyChangingEventHandler PropertyChanging;

}