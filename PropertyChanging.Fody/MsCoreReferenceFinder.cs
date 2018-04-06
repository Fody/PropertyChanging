using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference ComponentModelPropertyChangingEventHandlerInvokeReference;
    public MethodReference ComponentModelPropertyChangingEventConstructorReference;
    public MethodReference ActionConstructorReference;
    public MethodReference ObjectConstructor;
    public TypeReference ActionTypeReference;
    public MethodDefinition NullableEqualsMethod;
    public TypeReference PropChangingInterfaceReference;
    public TypeReference PropChangingHandlerReference;
    public MethodReference DelegateCombineMethodRef;
    public MethodReference DelegateRemoveMethodRef;
    public GenericInstanceMethod InterlockedCompareExchangeForPropChangingHandler;

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
        yield return "System";
        yield return "System.Runtime";
        yield return "System.Core";
        yield return "netstandard";
        yield return "System.Collections";
        yield return "System.ObjectModel";
        yield return "System.Threading";
        yield return "FSharp.Core";
    }

    public void FindCoreReferences()
    {
        var objectDefinition = FindType("System.Object");
        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = ModuleDefinition.ImportReference(constructorDefinition);

        var nullableDefinition = FindType("System.Nullable");
        NullableEqualsMethod = ModuleDefinition.ImportReference(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");

        var actionDefinition = FindType("System.Action");
        ActionTypeReference = ModuleDefinition.ImportReference(actionDefinition);

        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = ModuleDefinition.ImportReference(actionConstructor);

        var propChangingInterfaceDefinition = FindType("System.ComponentModel.INotifyPropertyChanging");
        var propChangingHandlerDefinition = FindType("System.ComponentModel.PropertyChangingEventHandler");
        var propChangingArgsDefinition = FindType("System.ComponentModel.PropertyChangingEventArgs");


        PropChangingInterfaceReference = ModuleDefinition.ImportReference(propChangingInterfaceDefinition);
        PropChangingHandlerReference = ModuleDefinition.ImportReference(propChangingHandlerDefinition);
        ComponentModelPropertyChangingEventHandlerInvokeReference = ModuleDefinition.ImportReference(propChangingHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        ComponentModelPropertyChangingEventConstructorReference = ModuleDefinition.ImportReference(propChangingArgsDefinition.Methods.First(x => x.IsConstructor));

        var delegateDefinition = FindType("System.Delegate");
        var combineMethodDefinition = delegateDefinition.Methods
            .Single(x =>
                x.Name == "Combine" &&
                x.Parameters.Count == 2 &&
                x.Parameters.All(p => p.ParameterType == delegateDefinition));
        DelegateCombineMethodRef = ModuleDefinition.ImportReference(combineMethodDefinition);
        var removeMethodDefinition = delegateDefinition.Methods.First(x => x.Name == "Remove");
        DelegateRemoveMethodRef = ModuleDefinition.ImportReference(removeMethodDefinition);

        var interlockedDefinition = FindType("System.Threading.Interlocked");
        var genericCompareExchangeMethodDefinition = interlockedDefinition
            .Methods.First(x =>
                x.IsStatic &&
                x.Name == "CompareExchange" &&
                x.GenericParameters.Count == 1 &&
                x.Parameters.Count == 3);
        var genericCompareExchangeMethod = ModuleDefinition.ImportReference(genericCompareExchangeMethodDefinition);

        InterlockedCompareExchangeForPropChangingHandler = new GenericInstanceMethod(genericCompareExchangeMethod);
        InterlockedCompareExchangeForPropChangingHandler.GenericArguments.Add(PropChangingHandlerReference);
    }
}