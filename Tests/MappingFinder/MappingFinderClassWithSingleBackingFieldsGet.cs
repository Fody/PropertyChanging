using System.Linq;
using Xunit;
// ReSharper disable ValueParameterNotUsed
#pragma warning disable 649

public class MappingFinderClassWithSingleBackingFieldsGet
{
    [Fact]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsGet>()).ToList();
        Assert.Equal("propertyA", memberMappings.First(_ => _.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.Equal("propertyB", memberMappings.First(_ => _.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
    }

    public class ClassWithSingleBackingFieldsGet
    {
        // ReSharper disable ConvertToAutoProperty
        string propertyA;

        public string Property1
        {
            get => propertyA;
            set { }
        }

        string propertyB;

        public string Property2
        {
            get => propertyB;
            set { }
        }
        // ReSharper restore ConvertToAutoProperty
    }
}