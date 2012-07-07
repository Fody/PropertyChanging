using System.ComponentModel;

public class ExperimentClass : INotifyPropertyChanging
{
    string property1;
    bool hasValue;

    public string Property1
    {
        get { return property1; }
        set
        {
            if (hasValue)
            {
                property1 = value;
            }
            else
            {
                property1 = value;
            }
        }
    }
    public event PropertyChangingEventHandler PropertyChanging;



}