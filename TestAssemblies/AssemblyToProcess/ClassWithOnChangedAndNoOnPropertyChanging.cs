using System.ComponentModel;

public class ClassWithOnChangedAndNoOnPropertyChanging : INotifyPropertyChanging
{
    public int OnProperty1ChangingCalled;
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            OnProperty1Changing();
        }
    }

    public void OnProperty1Changing()
    {
        OnProperty1ChangingCalled++;
    }

    public event PropertyChangingEventHandler PropertyChanging;
}