﻿using Fody;
using Xunit;

public class AssemblyWithBadNamedInvokerTests
{
    //TODO
   // [Fact]
    public void WithOnNotify()
    {
        var weavingTask = new ModuleWeaver();
        var weavingException = Assert.Throws<WeavingException>(() =>
        {
            weavingTask.ExecuteTestRun("AssemblyInheritingBadNamedInvoker.dll");
        });
        Assert.Equal("Could not inject EventInvoker method on type 'ChildClass'. It is possible you are inheriting from a base class and have not correctly set 'EventInvokerNames' or you are using a explicit PropertyChanged event and the event field is not visible to this instance. Either correct 'EventInvokerNames' or implement your own EventInvoker on this class. If you want to suppress this place a [DoNotNotifyAttribute] on ChildClass.", weavingException.Message);

    }
}
