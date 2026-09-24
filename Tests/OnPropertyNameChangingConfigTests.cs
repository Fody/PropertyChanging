using System.Xml.Linq;

public class OnPropertyNameChangingConfigTests
{
    [Test]
    public async Task False()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanging='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveOnPropertyNameChangingConfig();
        await Assert.That(moduleWeaver.InjectOnPropertyNameChanging).IsFalse();
    }

    [Test]
    public async Task True()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanging='true'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveOnPropertyNameChangingConfig();
        await Assert.That(moduleWeaver.InjectOnPropertyNameChanging).IsTrue();
    }

    [Test]
    public async Task Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveOnPropertyNameChangingConfig();
        await Assert.That(moduleWeaver.InjectOnPropertyNameChanging).IsTrue();
    }
}