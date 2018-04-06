using System.ComponentModel;
#pragma warning disable 649

public class ClassWithForwardedEvent : INotifyPropertyChanging
{
    InnerClass inner;

    public event PropertyChangingEventHandler PropertyChanging
    {
        add => inner.PropertyChanging += value;
        remove => inner.PropertyChanging -= value;
    }
}