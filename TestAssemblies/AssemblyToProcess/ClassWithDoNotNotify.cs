using System.ComponentModel;
using PropertyChanging;

[DoNotNotify]
public class ClassWithDoNotNotify : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
}