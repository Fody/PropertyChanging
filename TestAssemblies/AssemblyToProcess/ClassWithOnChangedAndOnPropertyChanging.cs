using System.ComponentModel;

public class ClassWithOnChangedAndOnPropertyChanging : INotifyPropertyChanging
{
    public int OnProperty1ChangingCalled;
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            OnPropertyChanging("Property1");
            OnProperty1Changing();
        }
    }

    public void OnProperty1Changing()
    {
        OnProperty1ChangingCalled++;
    }

    public event PropertyChangingEventHandler PropertyChanging;
    public virtual void OnPropertyChanging(string propertyName)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }
}