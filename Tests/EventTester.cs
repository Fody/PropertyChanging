using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xunit;

public static class EventTester
{
    internal static void TestPropertyNotCalled(dynamic instance)
    {
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = "a";
        Assert.False(property1EventCalled);
    }

    internal static void TestProperty(dynamic instance, bool checkProperty2)
    {
        var property1EventCalled = false;
        var property2EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }

            if (args.PropertyName == "Property2")
            {
                property2EventCalled = true;
            }
        };
        instance.Property1 = "a";

        Assert.True(property1EventCalled);
        if (checkProperty2)
        {
            Assert.True(property2EventCalled);
        }

        property1EventCalled = false;
        property2EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.False(property1EventCalled);
        if (checkProperty2)
        {
            Assert.False(property2EventCalled);
        }
    }

    internal static void TestProperty<T>(dynamic instance, string propertyName, T propertyValue)
    {
        var eventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == propertyName)
            {
                eventCalled = true;
            }
        };

        var type = (Type)instance.GetType();
        var propertyInfo = type.GetProperties().First(x => x.Name == propertyName);
        propertyInfo.SetValue(instance, propertyValue, null);

        Assert.True(eventCalled);
        eventCalled = false;
        propertyInfo.SetValue(instance, propertyValue, null);
        Assert.False(eventCalled);
    }

    public static dynamic GetInstance(this Assembly assembly, string className)
    {
        var type = assembly.GetType(className, true);
        return Activator.CreateInstance(type);
    }
}