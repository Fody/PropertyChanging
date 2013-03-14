using PropertyChanging;

[ImplementPropertyChanging]
public class ClassWithNotifyPropertyChangingAttributeChild : ClassWithNotifyPropertyChangingAttributeChildParent
{
    public string Property1 { get; set; }
}