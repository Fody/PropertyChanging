// ReSharper disable UnusedParameter.Local

using System.Linq;

// ReSharper disable ValueParameterNotUsed

public class AlreadyNotifyFinderTest
{
    [Test]
    public async Task ContainsNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotificationProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging");
        await Assert.That(propertyNames).HasSingleItem();
    }

    [Test]
    public async Task MultipleNotifications()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Multiple().Property);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging").ToList();
        await Assert.That(propertyNames).Contains("Property1");
        await Assert.That(propertyNames).Contains("Property2");
    }

    [Test]
    public async Task WithoutNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithoutNotificationProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging");
        await Assert.That(propertyNames).IsEmpty();
    }

    [Test]
    public async Task AlreadyContainsNotificationVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Virtual().WithNotificationProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging");
        await Assert.That(propertyNames).IsNotEmpty();
    }

    [Test]
    public async Task AlreadyContainsNotificationNonVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotificationProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging");
        await Assert.That(propertyNames).IsNotEmpty();
    }

    public class NonVirtual
    {
        public int WithoutNotificationProperty { get; set; }

        public int WithNotificationProperty
        {
            get => 0;
            set => OnPropertyChanging("WithNotificationProperty");
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        void OnPropertyChanging(string property)
        {
        }
    }

    public class Multiple
    {
        public int Property
        {
            get => 0;
            set
            {
                OnPropertyChanging("Property1");
                OnPropertyChanging("Property2");
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        void OnPropertyChanging(string property)
        {
        }
    }

    public class Virtual
    {
        public int WithoutNotificationProperty { get; set; }

        public int WithNotificationProperty
        {
            get => 0;
            set => OnPropertyChanging("WithNotificationProperty");
        }

        public virtual void OnPropertyChanging(string property)
        {
        }
    }
}
// ReSharper restore UnusedParameter.Local