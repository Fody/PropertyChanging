using System.ComponentModel;

public class ClassWithExplicitPropertyChanging : INotifyPropertyChanging
{
    PropertyChangingEventHandler propertyChanging;
    event PropertyChangingEventHandler INotifyPropertyChanging.PropertyChanging
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