using System.ComponentModel;

public class ClassWithOnChanging : INotifyPropertyChanging
{
    public bool OnProperty1ChangingCalled;

    public string Property1 { get; set; }
    public void OnProperty1Changing ()
    {
        OnProperty1ChangingCalled = true;
    }

    public event PropertyChangingEventHandler PropertyChanging;
}