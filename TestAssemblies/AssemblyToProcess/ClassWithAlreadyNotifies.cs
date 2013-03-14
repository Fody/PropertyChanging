using System.ComponentModel;

public class ClassWithAlreadyNotifies : INotifyPropertyChanging
{
    string property1;
    public string Property1
    {
        get { return property1; }
        set
        {
            OnPropertyChanging("Property1");
			OnPropertyChanging("Property2");
            property1 = value;
        }
    }

    public string Property2 { get { return Property1; } }
	public event PropertyChangingEventHandler PropertyChanging;

	void OnPropertyChanging(string propertyName)
    {
        var handler = PropertyChanging;
        if (handler != null)
        {
            handler(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}