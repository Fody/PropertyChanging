using System.ComponentModel;

public class ClassWithBranchingReturnAndNoField : INotifyPropertyChanging
{
    int x;
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