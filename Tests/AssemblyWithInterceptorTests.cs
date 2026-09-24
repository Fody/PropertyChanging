using System.Reflection;

public class AssemblyWithInterceptorTests
{
    [Test]
    public async Task Simple()
    {
        var weaver = new ModuleWeaver();
        var testResult = weaver.ExecuteTestRun("AssemblyWithInterceptor.dll",
            ignoreCodes: new[] {"0x80131869"});

        var assembly = testResult.Assembly;
        var instance = assembly.GetInstance("ClassToTest");
        await EventTester.TestProperty(instance, false);
        var type = assembly.GetType("PropertyChangingNotificationInterceptor");
        var propertyInfo = type.GetProperty("InterceptCalled", BindingFlags.Static | BindingFlags.Public);
        var value = (bool)propertyInfo.GetValue(null, null);
        await Assert.That(value).IsTrue();
    }

    [Test]
    public async Task Before()
    {
        var weaver = new ModuleWeaver();
        var testResult = weaver.ExecuteTestRun("AssemblyWithBeforeInterceptor.dll",
            ignoreCodes: new[] {"0x80131869"});
        var assembly = testResult.Assembly;
        var instance = assembly.GetInstance("ClassToTest");
        await EventTester.TestProperty(instance, false);
        var type = assembly.GetType("PropertyChangingNotificationInterceptor");
        var propertyInfo = type.GetProperty("InterceptCalled", BindingFlags.Static | BindingFlags.Public);
        var value = (bool)propertyInfo.GetValue(null, null);
        await Assert.That(value).IsTrue();
    }
}