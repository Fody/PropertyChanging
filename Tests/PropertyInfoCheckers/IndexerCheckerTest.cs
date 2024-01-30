using System.Linq;

// ReSharper disable ValueParameterNotUsed

public class IndexerCheckerTest
{
    [Fact]
    public void IsIndexer()
    {
        var checker = new ModuleWeaver();
        var propertyDefinition = DefinitionFinder.FindType<IndexerClass>().Properties.First();

        var message = checker.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.String);
        Assert.NotNull(message);
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