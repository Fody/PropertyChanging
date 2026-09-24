using System.Linq;

#pragma warning disable 649

public class MappingFinderClassWithInnerClass
{
    [Test]
    public async Task Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<Model>()).ToList();
        await Assert.That(memberMappings.First().FieldDefinition).IsNull();
    }

    public class Model
    {
        InnerClass innerClass;
        public string Property1
        {
            get => innerClass.Property1;
            set => innerClass.Property1 = value;
        }
    }

    class InnerClass
    {
        public string Property1 { get; set; }
    }
}