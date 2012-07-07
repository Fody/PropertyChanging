using System.ComponentModel;

public sealed class ClassThatIsSealed : INotifyPropertyChanging
{
    public string Property1 { get; set; }
    public event PropertyChangingEventHandler PropertyChanging;
}