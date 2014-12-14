using System.Xml.Linq;
using NUnit.Framework;

[TestFixture]
public class OnPropertyNameChangingConfigTests
{
    [Test]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanging='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveOnPropertyNameChangingConfig();
        Assert.IsFalse(moduleWeaver.InjectOnPropertyNameChanging);
    }

    [Test]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanging='true'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveOnPropertyNameChangingConfig();
        Assert.IsTrue(moduleWeaver.InjectOnPropertyNameChanging);
    }

    [Test]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveOnPropertyNameChangingConfig();
        Assert.IsTrue(moduleWeaver.InjectOnPropertyNameChanging);
    }

}