using Fody;
using Xunit;

public class AssemblyUsingAttributeWrongTests
{
    //TODO
    //[Fact]
    public void Foo()
    {
        var weavingTask = new ModuleWeaver();
        Assert.Throws<WeavingException>(() =>
        {
            weavingTask.ExecuteTestRun("AssemblyUsingAttributeWrong.dll");
        });
    }
}