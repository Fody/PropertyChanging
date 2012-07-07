using System.ComponentModel;

public class ClassWithOnChangingBerfore : INotifyPropertyChanging
{
    public bool OnProperty1ChangingCalled;

    public string Property1 { get; set; }
    public void OnProperty1Changing ()
    {
        OnProperty1ChangingCalled = true;
    }
    public void OnPropertyChanging(string propertyName, object before)
    {
        PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
    }

    public event PropertyChangingEventHandler PropertyChanging;
}