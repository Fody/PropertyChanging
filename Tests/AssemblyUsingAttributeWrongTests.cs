public class AssemblyUsingAttributeWrongTests
{
    //TODO
    //[Fact]
    public void Foo()
    {
        var weaver = new ModuleWeaver();
        Assert.Throws<WeavingException>(() =>
        {
            weaver.ExecuteTestRun("AssemblyUsingAttributeWrong.dll");
        });
    }
}