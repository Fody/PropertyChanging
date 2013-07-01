using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public class ClassWithGenericAndLambda<T> : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;

    public string Property1 { get; set; }

    public void MethodWithLambda(object data)
    {
        var list = new List<object>();
        list.First(container => container == data);
    }
}
public class ClassWithGenericAndLambdaImp : ClassWithGenericAndLambda<object>
{
    
}