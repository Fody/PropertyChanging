// ReSharper disable UnusedParameter.Local

using System.Linq;

// ReSharper disable ValueParameterNotUsed

public class AlreadyNotifyFinderTest
{
    [Fact]
    public void ContainsNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotificationProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging");
        Assert.Single(propertyNames);
    }

    [Fact]
    public void MultipleNotifications()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Multiple().Property);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging").ToList();
        Assert.Contains("Property1", propertyNames);
        Assert.Contains("Property2", propertyNames);
    }

    [Fact]
    public void WithoutNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithoutNotificationProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging");
        Assert.Empty(propertyNames);
    }

    [Fact]
    public void AlreadyContainsNotificationVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Virtual().WithNotificationProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging");
        Assert.NotEmpty(propertyNames);
    }

    [Fact]
    public void AlreadyContainsNotificationNonVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotificationProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanging");
        Assert.NotEmpty(propertyNames);
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