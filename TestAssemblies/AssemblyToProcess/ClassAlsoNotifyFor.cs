using System.ComponentModel;
using PropertyChanging;

public class ClassAlsoNotifyFor : INotifyPropertyChanging
{
    [AlsoNotifyFor("Property2")]
    public string Property1 { get; set; }
    public string Property2 { get; set; }
    public event PropertyChangingEventHandler PropertyChanging;
}