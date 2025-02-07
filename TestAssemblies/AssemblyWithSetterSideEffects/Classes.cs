using System.ComponentModel;

public class WithSideEffectBeforeValueAssignment :
    INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }

    string _property1;
    public string Property1
    {
        get => _property1;
        set
        {
            CallSideEffectBefore();
            _property1 = value;
        }
    }

    
    public int SideEffectBeforeCallCount { get; set; }
    void CallSideEffectBefore() => SideEffectBeforeCallCount++;
}

public class WithSideEffectAfterValueAssignment :
    INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }



    string _property1;
    public string Property1
    {
        get => _property1;
        set
        {
            _property1 = value;
            CallSideEffectAfter();
        }
    }

    public int SideEffectAfterCallCount { get; set; }
    void CallSideEffectAfter() => SideEffectAfterCallCount++;
}