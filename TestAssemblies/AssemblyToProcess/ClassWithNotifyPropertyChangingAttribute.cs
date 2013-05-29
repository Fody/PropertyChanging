using PropertyChanging;

[ImplementPropertyChanging]
public class ClassWithNotifyPropertyChangingAttribute
{
    public string Property1 { get; set; }
}

[ImplementPropertyChanging]
public class ClassWithNotifyPropertyChangingAttributeGeneric<T>
{
    public string Property1 { get; set; }
}