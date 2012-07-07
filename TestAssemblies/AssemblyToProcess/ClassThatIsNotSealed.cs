using System.ComponentModel;

public class ClassThatIsNotSealed : INotifyPropertyChanging
{
    public string Property1 { get; set; }
    public event PropertyChangingEventHandler PropertyChanging;
}