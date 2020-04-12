using System.ComponentModel;

public class ClassWithNullableBackingField : INotifyPropertyChanging
{
    // Issue 156

    bool? _isFlag;

    public bool IsFlag
    {
        get => _isFlag ?? (_isFlag = GetFlag()).Value;
        set => _isFlag = value;
    }

    bool GetFlag() => false;

    public event PropertyChangingEventHandler PropertyChanging;
}
