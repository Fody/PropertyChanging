using System.Collections.ObjectModel;
using System.ComponentModel;

public class A :
    ObservableCollection<string>;

public class B :
    INotifyPropertyChanging
{
    public string Property1 { get; set; }

    public event PropertyChangingEventHandler PropertyChanging;

    public void OnPropertyChanging(string propertyName)
    {
        PropertyChanging(this, new(propertyName));
    }
}