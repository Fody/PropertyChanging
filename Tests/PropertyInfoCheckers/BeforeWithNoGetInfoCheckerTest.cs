

// ReSharper disable ConvertToAutoPropertyWhenPossible

public class BeforeAfterWithNoGetInfoCheckerTest
{
    [Test]
    public async Task WithGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetInfoCheckerTest>("PropertyWithGet");

        var message = checker.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.Before);
        await Assert.That(message).IsNull();
    }

    [Test]
    public async Task NoGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetInfoCheckerTest>("PropertyNoGet");

        var message = checker.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.Before);
        await Assert.That(message).IsNotNull();
    }

    string property;

    internal string PropertyNoGet
    {
        set => property = value;
    }

    public string PropertyWithGet
    {
        set => property = value;
        get => property;
    }

}