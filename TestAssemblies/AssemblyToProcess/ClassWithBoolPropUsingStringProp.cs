using System.ComponentModel;

public class ClassWithBoolPropUsingStringProp: INotifyPropertyChanging
{
    public bool StringCompareProperty => StringProperty == "magicString";

    public bool BoolProperty { get; set; }

    string stringProperty;
    public string StringProperty
    {
        get { return stringProperty; }
        set
        {
            stringProperty = value;
            if (StringCompareProperty)
            {
                BoolProperty = true;
            }
        }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}