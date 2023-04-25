using System.ComponentModel;

public class ClassWithOnChangingBefore : INotifyPropertyChanging
{
    public bool OnProperty1ChangingCalled;

    public string Property1 { get; set; }
    public void OnProperty1Changing ()
    {
        OnProperty1ChangingCalled = true;
    }
    public void OnPropertyChanging(string propertyName, object before)
    {
        PropertyChanging(this, new(propertyName));
    }

    public event PropertyChangingEventHandler PropertyChanging;
}