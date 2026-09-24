
// ReSharper disable ValueParameterNotUsed

public class CheckForEqualityWithNoBackingFieldSetInfoCheckerTest
{
    [Test]
    public async Task WithBackingField()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty(() => WithBackingFieldProperty);

        var warning = checker.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
                BackingFieldReference = propertyDefinition.DeclaringType.Fields[0]
            },
            InvokerTypes.String);
        await Assert.That(warning).IsNull();
    }

    [Test]
    public async Task WithoutBackingField()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<CheckForEqualityWithNoBackingFieldSetInfoCheckerTest>("WithoutBackingFieldProperty");

        var warning = checker.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
                BackingFieldReference = null,
            },
            InvokerTypes.String);
        await Assert.That(warning).IsNotNull();
    }

    public int WithBackingFieldProperty { get; set; }

    internal int WithoutBackingFieldProperty
    {
        set { }
    }
}