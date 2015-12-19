using System.ComponentModel;

public class BaseClass : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
    public void OnPropertyChanging2(string text1)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(text1));
    }

}