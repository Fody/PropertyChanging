using System;
using System.ComponentModel;
#pragma warning disable 649

public class ClassWithBranchingReturn1 : INotifyPropertyChanging
{
    string property1;
    bool isInSomeMode;

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

    public event PropertyChangingEventHandler PropertyChanging;
}