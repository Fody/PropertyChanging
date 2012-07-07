using System.ComponentModel;
using PropertyChanging;

public class ClassCircularProperties : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
    [DependsOn("Self")]
    public string Self { get; set; }

    [DependsOn("PropertyA2")]
    public string PropertyA1 { get; set; }

    [DependsOn("PropertyA1")]
    public string PropertyA2 { get; set; }

    [AlsoNotifyFor ("PropertyB2")]
    public string PropertyB1 { get; set; }

    [AlsoNotifyFor ("PropertyB1")]
    public string PropertyB2 { get; set; }
}