using System.ComponentModel;

public class ClassWithCustomPropertyChanging : INotifyPropertyChanging
{
    PropertyChangingEventHandler propertyChanging;

    public event PropertyChangingEventHandler PropertyChanging
    {
        add
        {
            propertyChanging += value;
        }
        remove
        {
            propertyChanging -= value;
        }
    }

    public string Property1 { get; set; }

}