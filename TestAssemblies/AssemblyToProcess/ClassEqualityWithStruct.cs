using System.ComponentModel;

public class ClassEqualityWithStruct : INotifyPropertyChanging
{
    public SimpleStruct Property1 { get; set; }

    public struct SimpleStruct
    {
    }

    public event PropertyChangingEventHandler PropertyChanging;
}