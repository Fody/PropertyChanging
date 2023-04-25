using System.ComponentModel;
using PropertyChanging;

public class ClassWithBeforeAndSimpleImplementation : INotifyPropertyChanging
{

    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;

    public void OnPropertyChanging(string propertyName)
    {

    }
    public void OnPropertyChanging(string propertyName, object before)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }
}