using System.ComponentModel;
using PropertyChanging;

public class ClassAlsoNotifyForMultiple : INotifyPropertyChanging
{
    [AlsoNotifyFor("Property2", "Property3")]
    public string Property1 { get; set; }
    public string Property2 { get; set; }
    public string Property3 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;
}