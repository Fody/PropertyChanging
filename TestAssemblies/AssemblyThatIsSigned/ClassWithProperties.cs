using System.ComponentModel;


public class ClassWithProperties : INotifyPropertyChanging
{
    public string Property1 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;

}
