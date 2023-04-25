using System.ComponentModel;

public class ClassWithStackOverflow : INotifyPropertyChanging
{
    string name;

    public event PropertyChangingEventHandler PropertyChanging;

    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(name))
            {
                Name = "test";
            }

            return Name;
        }
        set { name = value; }
    }

    public string ValidName { get; set; }

    protected void OnPropertyChanging(string propertyName, object before)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }
}