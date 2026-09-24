using System.Linq;
using System.Reflection;

public static class EventTester
{
    internal static async Task TestPropertyNotCalled(dynamic instance)
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
        await Assert.That(property1EventCalled).IsFalse();
    }

    internal static async Task TestProperty(dynamic instance, bool checkProperty2)
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

        await Assert.That(property1EventCalled).IsTrue();
        if (checkProperty2)
        {
            await Assert.That(property2EventCalled).IsTrue();
        }

        property1EventCalled = false;
        property2EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        await Assert.That(property1EventCalled).IsFalse();
        if (checkProperty2)
        {
            await Assert.That(property2EventCalled).IsFalse();
        }
    }

    internal static async Task TestProperty<T>(dynamic instance, string propertyName, T propertyValue)
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
        var propertyInfo = type.GetProperties().First(_ => _.Name == propertyName);
        propertyInfo.SetValue(instance, propertyValue, null);

        await Assert.That(eventCalled).IsTrue();
        eventCalled = false;
        propertyInfo.SetValue(instance, propertyValue, null);
        await Assert.That(eventCalled).IsFalse();
    }

    public static dynamic GetInstance(this Assembly assembly, string className)
    {
        var type = assembly.GetType(className, true);
        return Activator.CreateInstance(type);
    }
}