// ReSharper disable UnusedParameter.Local

using System.Linq;
using NUnit.Framework;

[TestFixture]
public class AlreadyNotifyFinderTest
{


    [Test]
    public void ContainsNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotifactionProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging");
        Assert.AreEqual(1, propertyNames.Count());
    }
    [Test]
    public void MultipleNotifications()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Multiple().Property);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging").ToList();
        Assert.Contains("Property1", propertyNames);
        Assert.Contains("Property2", propertyNames);
    }

    [Test]
    public void WithoutNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithoutNotificationProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanged");
        Assert.IsEmpty(propertyNames);
    }


    [Test]
    public void AlreadyContainsNotificationVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Virtual().WithNotifactionProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging");
        Assert.IsNotEmpty(propertyNames);
    }

    [Test]
    public void AlreadyContainsNotificationNonVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotifactionProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging");
        Assert.IsNotEmpty(propertyNames);
    }

    public class NonVirtual
    {
        public int WithoutNotificationProperty { get; set; }

        public int WithNotifactionProperty
        {
            get
            {
                return 0;
            }
            set
            {
                OnPropertyChanging("WithNotifactionProperty");
            }
        }

        void OnPropertyChanging(string property) { }

    }
    public class Multiple
    {

        public int Property
        {
            get
            {
                return 0;
            }
            set
            {
                OnPropertyChanging("Property1");
                OnPropertyChanging("Property2");
            }
        }

        void OnPropertyChanging(string property) { }
    }
    public class Virtual
    {
        public int WithoutNotificationProperty { get; set; }

        public int WithNotifactionProperty
        {
            get
            {
                return 0;
            }
            set
            {
                OnPropertyChanging("WithNotifactionProperty");
            }
        }

        public virtual void OnPropertyChanging(string property)
        {


        }
    }
}
// ReSharper restore UnusedParameter.Local