using System.ComponentModel;

public class ExperimentClass : INotifyPropertyChanging
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            OnPropertyChanging<string>(ref property1, value, "Property1");
        }
    }
    public event PropertyChangingEventHandler PropertyChanging;
    protected bool OnPropertyChanging<T>(ref T storage, T value, string propertyName = null)
    {
        if (Equals(storage, value)) return false;

        storage = value;

        return true;
    }



}