
namespace GenericChildWithPropertyOnChanging;

public class ClassWithGenericPropertyChild : ClassWithGenericPropertyParent<string>
{
    public bool OnProperty1ChangingCalled;

    public string Property1 { get; set; }
    public void OnProperty1Changing()
    {
        OnProperty1ChangingCalled = true;
    }

    public bool OnProperty2ChangingCalled;

    public string Property2 { get; set; }
    public void OnProperty2Changing()
    {
        OnProperty2ChangingCalled = true;
    }
}