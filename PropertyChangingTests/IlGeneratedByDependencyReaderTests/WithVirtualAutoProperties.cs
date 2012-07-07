using System.Linq;
using NUnit.Framework;

[TestFixture]
public class WithVirtualAutoProperties
{
    //TODO: add test for abstract

    [Test]
    public void Run()
    {
        var typeDefinition = DefinitionFinder.FindType<Person>();
        var node = new TypeNode
                       {
                           TypeDefinition = typeDefinition,
                           Mappings = MappingFinder.GetMappings(typeDefinition).ToList()
                       };
        new IlGeneratedByDependencyReader(node).Process();
        var first = node.PropertyDependencies[0];
        Assert.AreEqual("FullName", first.ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("GivenNames", first.WhenPropertyIsSet.Name);
    }

    public class Person
    {
        public virtual string GivenNames { get; set; }
        public virtual string FullName
        {
            get
            {
                return GivenNames;
            }
        }
    }
}