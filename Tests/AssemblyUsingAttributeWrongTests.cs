using Fody;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class AssemblyUsingAttributeWrongTests :
    VerifyBase
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

    public AssemblyUsingAttributeWrongTests(ITestOutputHelper output) :
        base(output)
    {
    }
}