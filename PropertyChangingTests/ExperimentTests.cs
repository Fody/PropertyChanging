﻿using NUnit.Framework;

[TestFixture]
public class ExperimentTests
{
    [Test]
    [Ignore]
    public void Foo()
    {
        var weaverHelper = new WeaverHelper(@"AssemblyExperiments\AssemblyExperiments.csproj");
        var instance = weaverHelper.Assembly.GetInstance("ExperimentClass");
        EventTester.TestProperty(instance, false);
    }
}