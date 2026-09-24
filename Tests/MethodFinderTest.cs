using System.Linq;
using Mono.Cecil;

public class MethodFinderTest
{
    TypeDefinition typeDefinition;
    ModuleWeaver methodFinder;

    public MethodFinderTest()
    {
        var codeBase = typeof(MethodFinderTest).Assembly.Location;
        var module = ModuleDefinition.ReadModule(codeBase);
        methodFinder = new()
        {
            ModuleDefinition = module
        };

        typeDefinition = module.Types.First(_ => _.Name.EndsWith("MethodFinderTest"));
    }

    [Test]
    public async Task WithStringParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WithStringParam");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        await Assert.That(methodReference).IsNotNull();
        await Assert.That(methodReference.MethodReference.Name).IsEqualTo("OnPropertyChanging");
    }

    public class WithStringParam
    {
        public void OnPropertyChanging(string propertyName)
        {
        }
    }

    [Test]
    public async Task WithStringAndBeforeParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WithStringAndBefore");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        await Assert.That(methodReference).IsNotNull();
        await Assert.That(methodReference.MethodReference.Name).IsEqualTo("OnPropertyChanging");
        await Assert.That(methodReference.InvokerType).IsEqualTo(InvokerTypes.Before);
    }

    public class WithStringAndBefore
    {
        public void OnPropertyChanging(string propertyName, object before)
        {
        }
    }


    [Test]
    public async Task NoMethodTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "NoMethod");
        await Assert.That(methodFinder.RecursiveFindEventInvoker(definitionToProcess)).IsNull();
    }

    public class NoMethod;

    [Test]
    public async Task NoParamsTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "NoParams");
        await Assert.That(methodFinder.RecursiveFindEventInvoker(definitionToProcess)).IsNull();
    }

    public class NoParams
    {
        public void OnPropertyChanging()
        {
        }
    }

    [Test]
    public async Task WrongParamsTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WrongParams");
        await Assert.That(methodFinder.RecursiveFindEventInvoker(definitionToProcess)).IsNull();
    }

    public class WrongParams
    {
        public void OnPropertyChanging(int propertyName)
        {
        }
    }
}