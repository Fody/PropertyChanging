public class AssemblyWithBaseInDifferentModuleTests
{
    TestResult testResult;

    public AssemblyWithBaseInDifferentModuleTests()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun("AssemblyWithBaseInDifferentModule.dll", ignoreCodes: new[] { "0x80131869" });
    }

    [Test]
    public async Task SimpleChildClass()
    {
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.Simple.ChildClass");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public async Task GenericChildClass()
    {
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.BaseWithGenericParent.ChildClass");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public async Task GenericFromAbove()
    {
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.GenericFromAbove.ChildClass");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public async Task DirectChildClass()
    {
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.DirectGeneric.ChildClass");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public async Task GenericChildClassFromMultiType()
    {
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.MultiTypes.ChildClass");
        await EventTester.TestProperty(instance, false);
    }
}