using NUnit.Framework;

[TestFixture]
public class AssemblyWithBadNamedInvokerTests
{
    [Test]
    public void WithOnNotify()
    {
        var weaverHelper = new WeaverHelper(@"AssemblyInheritingBadNamedInvoker\AssemblyInheritingBadNamedInvoker.csproj");
        var instance = weaverHelper.Assembly.GetInstance("ChildClass");
        //TODO: validate that a log message is written
        //TODO: move ClassWithForwardedEvent.cs into own project and do the same kind of test
        //EventTester.TestProperty(instance, false);
    }
}
