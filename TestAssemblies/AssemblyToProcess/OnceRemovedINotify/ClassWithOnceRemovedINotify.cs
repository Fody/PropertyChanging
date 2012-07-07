using System.ComponentModel;

public class ClassWithOnceRemovedINotify : INotifyPropertyChangingChild
{
    public string Property1 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;
}