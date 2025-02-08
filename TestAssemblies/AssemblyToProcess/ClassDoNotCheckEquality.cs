using System.ComponentModel;

public class ClassDoNotCheckEquality :
    INotifyPropertyChanging
{
    public int TimesProperty1Changing;
    public int TimesProperty2Changing;

    public string Property1 { get; set; }

    [PropertyChanging.DoNotCheckEquality]
    public string Property2 { get; set; }

    public void OnProperty1Changing()
    {
        TimesProperty1Changing += 1;
    }

    public void OnProperty2Changing()
    {
        TimesProperty2Changing += 1;
    }

    public event PropertyChangingEventHandler PropertyChanging;
}