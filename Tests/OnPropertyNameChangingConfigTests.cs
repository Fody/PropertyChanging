﻿using System.Xml.Linq;

public class OnPropertyNameChangingConfigTests
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanging='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveOnPropertyNameChangingConfig();
        Assert.False(moduleWeaver.InjectOnPropertyNameChanging);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanging='true'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveOnPropertyNameChangingConfig();
        Assert.True(moduleWeaver.InjectOnPropertyNameChanging);
    }

    [Fact]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveOnPropertyNameChangingConfig();
        Assert.True(moduleWeaver.InjectOnPropertyNameChanging);
    }
}