using System.ComponentModel;

public class ClassToTest : INotifyPropertyChanging
{
    public string Property1 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;

}
