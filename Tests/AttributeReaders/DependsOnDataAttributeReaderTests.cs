using PropertyChanging;

public class DependsOnDataAttributeReaderTests
{
    [Test]
    public async Task Integration()
    {
        var reader = new ModuleWeaver();
        var node = new TypeNode
        {
            TypeDefinition = DefinitionFinder.FindType<Person>()
        };
        reader.ProcessDependsOnAttributes(node);

        await Assert.That(node.PropertyDependencies[0].ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(node.PropertyDependencies[0].WhenPropertyIsSet.Name).IsEqualTo("GivenNames");
        await Assert.That(node.PropertyDependencies[1].ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(node.PropertyDependencies[1].WhenPropertyIsSet.Name).IsEqualTo("FamilyName");
    }

    public class Person
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        [PropertyChanging.DependsOn("GivenNames", "FamilyName")]
        public string FullName => $"{GivenNames} {FamilyName}";
    }

    [Test]
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

        [PropertyChanging.DependsOn("NotAProperty1", "NotAProperty2")]
        public string FullName => $"{GivenNames} {FamilyName}";
    }
}