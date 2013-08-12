using System.ComponentModel;
using PropertyChanging;

public class ClassWithPropertyChangingArgImplementation : INotifyPropertyChanging
{
    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;

    public virtual void OnPropertyChanging(PropertyChangingEventArgs propertyArg)
    {
        var handler = PropertyChanging;
        if (handler != null)
        {
            handler(this, propertyArg);
        }
    }

}