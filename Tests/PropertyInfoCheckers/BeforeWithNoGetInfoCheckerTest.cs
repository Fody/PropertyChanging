using Xunit;

// ReSharper disable ConvertToAutoPropertyWhenPossible

public class BeforeAfterWithNoGetInfoCheckerTest
{
    [Fact]
    public void WithGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetInfoCheckerTest>("PropertyWithGet");

        var message = checker.CheckForWarning(new PropertyData
        {
            PropertyDefinition = propertyDefinition,
        }, InvokerTypes.Before);
        Assert.Null(message);
    }

    [Fact]
    public void NoGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetInfoCheckerTest>("PropertyNoGet");

        var message = checker.CheckForWarning(new PropertyData
        {
            PropertyDefinition = propertyDefinition,
        }, InvokerTypes.Before);
        Assert.NotNull(message);
    }

    string property;

    public string PropertyNoGet
    {
        set => property = value;
    }
    public string PropertyWithGet
    {
        set => property = value;
        get => property;
    }

}