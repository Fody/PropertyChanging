using PropertyChanging;

[ImplementPropertyChanging]
public class ClassWithNotifyInChildByAttribute : ParentClass
{
    public string Property { get; set; }
}