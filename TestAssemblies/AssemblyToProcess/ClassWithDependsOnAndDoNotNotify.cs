using System.ComponentModel;
using PropertyChanging;

public class ClassWithDependsOnAndDoNotNotify : INotifyPropertyChanging
{
    [DoNotNotify]
    public string UseLessProperty { get; set; }
    public string Property1 { get; set; }    
    public string Property2 { get { return Property1; } }


    public event PropertyChangingEventHandler PropertyChanging;
}