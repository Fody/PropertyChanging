using System.ComponentModel;

public class ClassWithTernary : INotifyPropertyChanging
{
    decimal? property1;

    public decimal? Property1
    {
        get { return property1; }
        set
        {
            var newValue = value == 0.0m ? null : value;
            if (property1 != newValue)
            {
                property1 = newValue;
            }
        }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}