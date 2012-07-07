using System.ComponentModel;

public class ClassEqualityWithDouble : INotifyPropertyChanging
{
    public double Property1 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;
}