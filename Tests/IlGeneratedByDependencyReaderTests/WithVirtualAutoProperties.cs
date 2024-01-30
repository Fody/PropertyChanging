using System.Linq;

public class WithVirtualAutoProperties
{
    //TODO: add test for abstract

    [Fact]
    public void Run()
    {
        var typeDefinition = DefinitionFinder.FindType<Person>();
        var node = new TypeNode
                       {
                           TypeDefinition = typeDefinition,
                           Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
                       };
        new IlGeneratedByDependencyReader(node).Process();
        var first = node.PropertyDependencies[0];
        Assert.Equal("FullName", first.ShouldAlsoNotifyFor.Name);
        Assert.Equal("GivenNames", first.WhenPropertyIsSet.Name);
    }

    public class Person
    {
        public virtual string GivenNames { get; set; }
        public virtual string FullName => GivenNames;
    }
}