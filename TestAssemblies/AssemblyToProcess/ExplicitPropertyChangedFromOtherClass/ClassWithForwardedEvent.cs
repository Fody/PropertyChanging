using System.ComponentModel;

public class ClassWithForwardedEvent : INotifyPropertyChanging
{
    InnerClass inner;

    public event PropertyChangingEventHandler PropertyChanging
    {
        add { inner.PropertyChanging += value; }
        remove { inner.PropertyChanging -= value; }
    }

}
