﻿using NUnit.Framework;


[TestFixture]
public class TypeProcessorTest
{


    [Test]
    public void ContainsNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotifactionProperty);

        var message = TypeProcessor.AlreadyContainsNotification(propertyDefinition, "OnPropertyChanging");
        Assert.IsTrue(message);
    }

    [Test]
    public void WithoutNotification()
    {


        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithoutNotificationProperty);

        var message = TypeProcessor.AlreadyContainsNotification(propertyDefinition, "OnPropertyChanging");
        Assert.IsFalse(message);
    }


    [Test]
    public void AlreadyContainsNotificationVirtual()
    {

        var propertyDefinition = DefinitionFinder.FindProperty(() => new Virtual().WithNotifactionProperty);

        var message = TypeProcessor.AlreadyContainsNotification(propertyDefinition, "OnPropertyChanging");
        Assert.IsTrue(message);
    }

    [Test]
    public void AlreadyContainsNotificationNonVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotifactionProperty);

        var message = TypeProcessor.AlreadyContainsNotification(propertyDefinition, "OnPropertyChanging");
        Assert.IsTrue(message);
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
                OnPropertyChanging();
            }
        }

        void OnPropertyChanging()
        {


        }
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
                OnPropertyChanging();
            }
        }

        public virtual void OnPropertyChanging()
        {


        }
    }
}