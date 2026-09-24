public class FSharpTest
{
    TestResult testResult;

    public FSharpTest()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun("AssemblyFSharp.dll", runPeVerify: false);
    }

    [Test]
    public async Task SimpleClass()
    {
        var instance = testResult.GetInstance("Namespace.ClassWithProperties");
        await EventTester.TestProperty(instance, false);
    }
}