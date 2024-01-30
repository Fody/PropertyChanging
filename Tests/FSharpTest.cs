public class FSharpTest
{
    TestResult testResult;

    public FSharpTest()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun("AssemblyFSharp.dll", runPeVerify: false);
    }

    [Fact]
    public void SimpleClass()
    {
        var instance = testResult.GetInstance("Namespace.ClassWithProperties");
        EventTester.TestProperty(instance, false);
    }
}