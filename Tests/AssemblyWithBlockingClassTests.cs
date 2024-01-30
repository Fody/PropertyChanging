public class AssemblyWithBlockingClassTests
{
    [Fact]
    public void TestClassIsNotBlocked()
    {
        var weaver = new ModuleWeaver();
        var testResult = weaver.ExecuteTestRun("AssemblyWithBlockingClass.dll",
            ignoreCodes: new[] {"0x80131869"});
        var instance = testResult.GetInstance("B");
        EventTester.TestProperty(instance, false);
    }
}