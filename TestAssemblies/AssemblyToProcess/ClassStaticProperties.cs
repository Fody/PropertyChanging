using System.ComponentModel;

public class ClassStaticProperties : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
    public static string Property { get; set; }
}