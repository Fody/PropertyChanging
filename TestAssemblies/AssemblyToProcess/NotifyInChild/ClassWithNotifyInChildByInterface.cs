using System.ComponentModel;

public class ClassWithNotifyInChildByInterface : ParentClass, INotifyPropertyChanging
{
    public string Property { get; set; }
    public event PropertyChangingEventHandler PropertyChanging;
}