#if(DEBUG)

using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class PhoneWeavingTaskTests : BaseTaskTests
{

    public PhoneWeavingTaskTests()
        : base(@"AssemblyToProcess\AssemblyToProcessPhone.csproj")
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {


        if (args.Name.Contains("mscorlib.Extensions"))
        {
            var codeBase = GetType().Assembly.CodeBase;
            codeBase = codeBase.Substring(8, codeBase.Length - 8);
            var directoryName = Path.GetDirectoryName(codeBase);
            var combine =Path.GetFullPath( Path.Combine(directoryName, @"..\..\..\tools\WindowsPhone\mscorlib.Extensions.dll"));
            return Assembly.LoadFrom(combine);
        }
        return null;
    }
    public override void AlreadyHasNotifcation()
    {
    }
    public override void EqualityWithStruct()
    {
    }
    public override void EqualityWithStructOverload()
    {
    }
    public override void AlsoNotifyFor()
    {
    }
    public override void Child1()
    {
    }
    public override void WithFieldGetButNoFieldSet()
    {
    }
    public override void Child2()
    {
    }
    public override void Child3()
    {
    }
    public override void CircularProperties()
    {
    }
    public override void DependsOn()
    {
    }
    public override void EnsureOnly1RefToMscorLib()
    {
    }
    public override void EqualityWithDouble()
    {
    }
    public override void Equality()
    {
    }
    public override void GenericBaseWithProperty()
    {
    }
    public override void GenericBaseWithPropertyBefore()
    {
    }
    public override void GenericChildWithProperty()
    {
    }
    
    public override void GenericChildWithPropertyBefore()
    {
    }
    public override void HierachyBeforeAndSimple()
    {
    }
    public override void NoBackingEqualityField()
    {
    }
    public override void Nested()
    {
    }
    public override void SealedForSealed()
    {
    }
    public override void PeVerify()
    {
    }
    public override void NoBackingNoEqualityField()
    {
    }
    public override void TransitiveDependencies()
    {
    }
    public override void VirtualForNonSealed()
    {
    }
    public override void WithBeforeAndSimpleImplementation()
    {
    }
    public override void WithDependsOnAndDoNotNotify()
    {
    }
    public override void WithBeforeAfterImplementation()
    {
    }
    public override void UsingPublicFieldThroughParameter()
    {
    }
    public override void WithBranchingAndBeforeAfterReturn()
    {
    }
    public override void WithBoolPropUsingStringProp()
    {
    }
    public override void WithBranchingReturn1()
    {
    }
    public override void WithBranchingReturn2True()
    {
    }
    public override void WithBranchingReturn2False()
    {
    }
    public override void WithCompilerGeneratedAttribute()
    {
    }
    public override void WithCustomPropertyChanging()
    {
    }
    public override void WithDependencyAfterSet()
    {
    }
    public override void WithFieldFromOtherClass()
    {
    }
    public override void WithExplicitPropertyChanging()
    {
    }
    public override void WithGeneratedCodeAttribute()
    {
    }
    public override void WithGeneric()
    {
    }
    public override void WithIndexerClass()
    {
    }
    public override void WithLogicInSet()
    {
    }
    public override void WithNotifyInBase()
    {
    }
    public override void WithOnceRemovedINotify()
    {
    }
    public override void WithOnChanging()
    {
    }
    public override void WithOnChangingBerfore()
    {
    }
    public override void WithOwnImplementation()
    {
    }
    public override void WithPropertyImpOfAbstractProperty()
    {
    }
    public override void WithPropertySetInCatch()
    {
    }
    public override void WithTryCatchInSet()
    {
    }
    public override void WithGenericAmdLambda() { }
}
#endif