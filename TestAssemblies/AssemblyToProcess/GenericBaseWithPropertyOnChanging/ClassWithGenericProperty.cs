using System.ComponentModel;

namespace GenericBaseWithPropertyOnChanging;

public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanging
{
    public bool OnProperty1ChangingCalled;
    public bool OnProperty2ChangingCalled;

    public T Property1 { get; set; }
    public void OnProperty1Changing()
    {
        OnProperty1ChangingCalled = true;
    }

    public T Property2 { get; set; }
    public void OnProperty2Changing()
    {
        OnProperty2ChangingCalled = true;
    }

    public event PropertyChangingEventHandler PropertyChanging;
}