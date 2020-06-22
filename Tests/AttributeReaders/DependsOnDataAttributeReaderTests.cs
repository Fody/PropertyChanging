using PropertyChanging;
using Xunit;

public class DependsOnDataAttributeReaderTests
{
    [Fact]
    public void Integration()
    {
        var reader = new ModuleWeaver();
        var node = new TypeNode
        {
            TypeDefinition = DefinitionFinder.FindType<Person>()
        };
        reader.ProcessDependsOnAttributes(node);

        Assert.Equal("FullName", node.PropertyDependencies[0].ShouldAlsoNotifyFor.Name);
        Assert.Equal("GivenNames", node.PropertyDependencies[0].WhenPropertyIsSet.Name);
        Assert.Equal("FullName", node.PropertyDependencies[1].ShouldAlsoNotifyFor.Name);
        Assert.Equal("FamilyName", node.PropertyDependencies[1].WhenPropertyIsSet.Name);
    }

    public class Person
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        [DependsOn("GivenNames", "FamilyName")]
        public string FullName => $"{GivenNames} {FamilyName}";
    }

    [Fact]
    public void PropertyThatDoesNotExist()
    {
        var reader = new ModuleWeaver();
        var node = new TypeNode
        {
            TypeDefinition = DefinitionFinder.FindType<ClassWithInvalidDepends>(),
        };
        reader.ProcessDependsOnAttributes(node);
    }

    public class ClassWithInvalidDepends
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        [DependsOn("NotAProperty1", "NotAProperty2")]
        public string FullName => $"{GivenNames} {FamilyName}";
    }
}