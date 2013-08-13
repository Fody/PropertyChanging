using System.Linq;
using Mono.Cecil;
using NUnit.Framework;


[TestFixture]
public class MethodFinderTest
{

    TypeDefinition typeDefinition;
    ModuleWeaver methodFinder;

    public MethodFinderTest()
    {

        var codeBase = typeof(MethodFinderTest).Assembly.CodeBase.Replace("file:///", string.Empty);
        var module = ModuleDefinition.ReadModule(codeBase);
        methodFinder = new ModuleWeaver
        {
            ModuleDefinition = module
        };

        typeDefinition = module.Types.First(x => x.Name.EndsWith("MethodFinderTest"));
    }


    [Test]
    public void WithStringParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithStringParam");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        Assert.IsNotNull(methodReference);
        Assert.AreEqual("OnPropertyChanging", methodReference.MethodReference.Name);
    }

    public class WithStringParam
    {
        public void OnPropertyChanging(string propertyName)
        {
        }
    }


    [Test]
    public void WithStringAndBeforeParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithStringAndBefore");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        Assert.IsNotNull(methodReference);
        Assert.AreEqual("OnPropertyChanging", methodReference.MethodReference.Name);
        Assert.AreEqual(InvokerTypes.Before, methodReference.InvokerType);
    }

    public class WithStringAndBefore
    {
        public void OnPropertyChanging(string propertyName, object before)
        {
        }
    }


    [Test]
    public void NoMethodTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "NoMethod");
        Assert.IsNull(methodFinder.RecursiveFindEventInvoker(definitionToProcess));
    }

    public class NoMethod
    {
    }
    [Test]
    public void NoParamsTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "NoParams");
        Assert.IsNull(methodFinder.RecursiveFindEventInvoker(definitionToProcess));
    }

    public class NoParams
    {
        public void OnPropertyChanging()
        {
        }
    }

    [Test]
    public void WrongParamsTest()
    {

        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WrongParams");
        Assert.IsNull(methodFinder.RecursiveFindEventInvoker(definitionToProcess));
    }

    public class WrongParams
    {
        public void OnPropertyChanging(int propertyName)
        {
        }
    }

}
