using System.ComponentModel;
using PropertyChanging;

public class ClassDependsOn : INotifyPropertyChanging
{
    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;
}