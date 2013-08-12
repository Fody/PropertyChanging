﻿using NUnit.Framework;

[TestFixture]
public class PropertyChangingArgWithNoGetInfoCheckerTest
{

    [Test]
    public void WithGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangingArgWithNoGetInfoCheckerTest>("PropertyWithGet");

        var message = checker.CheckForWarning(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                }, InvokerTypes.PropertyChangingArg);
        Assert.IsNull(message);
    }

    [Test]
    public void NoGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangingArgWithNoGetInfoCheckerTest>("PropertyNoGet");

        var message = checker.CheckForWarning(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                }, InvokerTypes.PropertyChangingArg);
        Assert.IsNotNull(message);
    }




    string property;

    public string PropertyNoGet
    {
        set { property = value; }
    }
    public string PropertyWithGet
    {
        set { property = value; }
        get { return property; }
    }

}
