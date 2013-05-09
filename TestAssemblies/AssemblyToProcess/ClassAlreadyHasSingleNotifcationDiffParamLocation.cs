using System.ComponentModel;

public class ClassAlreadyHasSingleNotifcationDiffParamLocation : INotifyPropertyChanging
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            OnPropertyChanging(9, "Property1");
            property1 = value;
        }
    }

    public virtual void OnPropertyChanging(int fake, string propertyName)
    {
        var handler = PropertyChanging;
        if (handler != null)
        {
            handler(this, new PropertyChangingEventArgs(propertyName));
        }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}