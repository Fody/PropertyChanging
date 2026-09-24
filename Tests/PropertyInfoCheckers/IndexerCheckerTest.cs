using System.Linq;

// ReSharper disable ValueParameterNotUsed

public class IndexerCheckerTest
{
    [Test]
    public async Task IsIndexer()
    {
        var checker = new ModuleWeaver();
        var propertyDefinition = DefinitionFinder.FindType<IndexerClass>().Properties.First();

        var message = checker.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.String);
        await Assert.That(message).IsNotNull();
    }

    public abstract class IndexerClass
    {
        public string this[string i]
        {
            get => null;
            set { }
        }
    }
}