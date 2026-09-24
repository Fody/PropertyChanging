public class AssemblyUsingAttributeWrongTests
{
    //TODO
    //[Test]
    public async Task Foo()
    {
        var weaver = new ModuleWeaver();
        await Assert.That(() =>
        {
            weaver.ExecuteTestRun("AssemblyUsingAttributeWrong.dll");
        }).Throws<WeavingException>();
    }
}
