public class AssemblyWithBlockingClassTests
{
    [Test]
    public async Task TestClassIsNotBlocked()
    {
        var weaver = new ModuleWeaver();
        var testResult = weaver.ExecuteTestRun("AssemblyWithBlockingClass.dll",
            ignoreCodes: new[] {"0x80131869"});
        var instance = testResult.GetInstance("B");
        await EventTester.TestProperty(instance, false);
    }
}