﻿using NUnit.Framework;


[TestFixture]
public class AssemblyWithBaseInDifferentModuleTests
{

    WeaverHelper weaverHelper;
    public AssemblyWithBaseInDifferentModuleTests()
    {
        weaverHelper = new WeaverHelper(@"AssemblyWithBaseInDifferentModule\AssemblyWithBaseInDifferentModule.csproj");
    }

    [Test]
    public void SimpleChildClass()
    {
        var instance = weaverHelper.Assembly.GetInstance("AssemblyWithBaseInDifferentModule.Simple.ChildClass");
        EventTester.TestProperty(instance, false);
    }
    [Test]
    public void GenericChildClass()
    {
        var instance = weaverHelper.Assembly.GetInstance("AssemblyWithBaseInDifferentModule.BaseWithGenericParent.ChildClass");
        EventTester.TestProperty(instance, false);
    }
    [Test]
    public void GenericFromAbove()
    {
        var instance = weaverHelper.Assembly.GetInstance("AssemblyWithBaseInDifferentModule.GenericFromAbove.ChildClass");
        EventTester.TestProperty(instance, false);
    }
    [Test]
    public void DirectChildClass()
    {
        var instance = weaverHelper.Assembly.GetInstance("AssemblyWithBaseInDifferentModule.DirectGeneric.ChildClass");
        EventTester.TestProperty(instance, false);
    }
    [Test]
    public void GenericChildClassFromMultiType()
    {
        var instance = weaverHelper.Assembly.GetInstance("AssemblyWithBaseInDifferentModule.MultiTypes.ChildClass");
        EventTester.TestProperty(instance, false);
    }
    [Test]
    public void Verify()
    {
        Verifier.Verify(weaverHelper.Assembly.Location);
    }
}