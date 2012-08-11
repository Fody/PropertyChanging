using System.ComponentModel;

public class ClassWithExplicitPropertyChanging : INotifyPropertyChanging
{
// ReSharper disable NotAccessedField.Local
    PropertyChangingEventHandler propertyChanging;
// ReSharper restore NotAccessedField.Local
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