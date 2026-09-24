using System.Linq;

public class WithVirtualAutoProperties
{
    //TODO: add test for abstract

    [Test]
    public async Task Run()
    {
        var typeDefinition = DefinitionFinder.FindType<Person>();
        var node = new TypeNode
                       {
                           TypeDefinition = typeDefinition,
                           Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
                       };
        new IlGeneratedByDependencyReader(node).Process();
        var first = node.PropertyDependencies[0];
        await Assert.That(first.ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(first.WhenPropertyIsSet.Name).IsEqualTo("GivenNames");
    }

    public class Person
    {
        public virtual string GivenNames { get; set; }
        public virtual string FullName => GivenNames;
    }
}