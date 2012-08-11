using System.ComponentModel;

public class ClassWithBranchingReturnAndNoField : INotifyPropertyChanging
{
// ReSharper disable NotAccessedField.Local
    int x;
// ReSharper restore NotAccessedField.Local
    public bool HasValue;

    public string Property1
    {
        get { return null; }
        set
        {
            if (HasValue)
            {
                x++;
            }
            else
            {
                x++;
            }
        }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}