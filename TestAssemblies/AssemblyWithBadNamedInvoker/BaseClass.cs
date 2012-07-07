using System.ComponentModel;

public class BaseClass : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
    public void OnPropertyChanging2(string text1)
    {
        var handler = PropertyChanging;
        if (handler != null)
        {
            handler(this, new PropertyChangingEventArgs(text1));
        }
    }

}