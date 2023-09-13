using System;
using System.ComponentModel;
using System.Linq.Expressions;

public class ClassWithExpression : INotifyPropertyChanging
{
    public string Property1;

    public ClassWithExpression()
    {
        // ReSharper disable UnusedVariable
        Expression<Func<ClassWithExpression, string>> expressionFunc = _ => _.Property1;
        Func<ClassWithExpression, string> func = _ => _.Property1;
        Action<ClassWithExpression, string> expression2 = (expression, s) => { expression.Property1 = s; };
        // ReSharper restore UnusedVariable
    }

    public event PropertyChangingEventHandler PropertyChanging;
}