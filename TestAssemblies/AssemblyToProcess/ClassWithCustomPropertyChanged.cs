using System.ComponentModel;

public class ClassWithCustomPropertyChanging : INotifyPropertyChanging
{
// ReSharper disable NotAccessedField.Local
    PropertyChangingEventHandler propertyChanging;
// ReSharper restore NotAccessedField.Local

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