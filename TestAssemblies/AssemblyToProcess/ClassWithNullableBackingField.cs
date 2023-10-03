using System.ComponentModel;

public class ClassWithNullableBackingField :
    INotifyPropertyChanging
{
    // Issue 156

    bool? _isFlag;

    public bool IsFlag
    {
        get => _isFlag ?? (_isFlag = GetFlag()).Value;
        set => _isFlag = value;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    bool GetFlag() => false;

    public event PropertyChangingEventHandler PropertyChanging;
}
