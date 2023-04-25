using System.ComponentModel;

public class ClassWithBeforeImplementationMissingSetGet : INotifyPropertyChanging
{
    string property;

    public string PropertyNoSet => property;


    public string PropertyNoGet
    {
        set { property = value; }
    }

    public event PropertyChangingEventHandler PropertyChanging;

    public void OnPropertyChanging(string propertyName, object before)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }

}