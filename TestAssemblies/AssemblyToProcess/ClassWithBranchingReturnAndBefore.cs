using System;
using System.ComponentModel;

public class ClassWithBranchingReturnAndBefore : INotifyPropertyChanging
{
    string property1;
    bool isInSomeMode;
    public event PropertyChangingEventHandler PropertyChanging;

    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            if (isInSomeMode)
            {
                Console.WriteLine("code here so 'if' does not get optimized away in release mode");
// ReSharper disable RedundantJumpStatement
                return;
// ReSharper restore RedundantJumpStatement
            }
        }
    }

    public void OnPropertyChanging(string propertyName, object before)
    {
        var handler = PropertyChanging;
        if (handler != null)
        {
            handler(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}
