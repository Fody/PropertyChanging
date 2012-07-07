using System.ComponentModel;

public class ClassParentWithProperty : INotifyPropertyChanging
{
    public string Property2 { get; set; }
    public event PropertyChangingEventHandler PropertyChanging;
}