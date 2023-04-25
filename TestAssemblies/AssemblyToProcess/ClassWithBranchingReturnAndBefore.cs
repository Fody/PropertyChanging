using System;
using System.ComponentModel;
#pragma warning disable 649

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
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }
    }

    public void OnPropertyChanging(string propertyName, object before)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }
}
