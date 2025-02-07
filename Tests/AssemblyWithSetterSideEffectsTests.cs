using System.Diagnostics;

public class AssemblyWithSetterSideEffectsTests
{
    static readonly string[] peVerifyIgnoreCodes =
    {
#if NETCOREAPP
        "0x80131869"
#endif
    };

    const string assemblyName = "AssemblyWithSetterSideEffects.dll";
    
    [Theory]
    [InlineData([true, "same", "same", 2])]
    [InlineData([true, "different1", "different2", 2])]
    public void CallsPreAssignmentSideEffect(bool checkForEquality, string firstAssignment, string secondAssignment, int expectedCallCount)
    {
        const string className = "WithSideEffectBeforeValueAssignment";

        Debug.Print($"Using check for equality: {checkForEquality}");

        var weaver = new ModuleWeaver();
        var testResult = weaver.ExecuteTestRun(assemblyName, ignoreCodes: peVerifyIgnoreCodes);
        
        var instance = testResult.GetInstance(className);

        instance.Property1 = firstAssignment;
        instance.Property1 = secondAssignment;
        
        var callCount = (int)instance.SideEffectBeforeCallCount;
        Assert.Equal(expectedCallCount, callCount);
    }

    [Theory]
    [InlineData([true, "same", "same", 2])]
    [InlineData([true, "different1", "different2", 2])]
    public void CallsPostAssignmentSideEffect(bool checkForEquality, string firstAssignment, string secondAssignment, int expectedCallCount)
    {
        const string className = "WithSideEffectAfterValueAssignment";

        Debug.Print($"Using check for equality: {checkForEquality}");

        var weaver = new ModuleWeaver();
        var testResult = weaver.ExecuteTestRun(assemblyName, ignoreCodes: peVerifyIgnoreCodes);
        
        var instance = testResult.GetInstance(className);
        
        instance.Property1 = firstAssignment;
        instance.Property1 = secondAssignment;
        
        var callCount = (int)instance.SideEffectAfterCallCount;
        Assert.Equal(expectedCallCount, callCount);
    }
}