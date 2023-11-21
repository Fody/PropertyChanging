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
        var objectDefinition = FindTypeDefinition("System.Object");
        var constructorDefinition = objectDefinition.Methods.First(_ => _.IsConstructor);
        ObjectConstructor = ModuleDefinition.ImportReference(constructorDefinition);

        var nullableDefinition = FindTypeDefinition("System.Nullable");
        NullableEqualsMethod = ModuleDefinition.ImportReference(nullableDefinition).Resolve().Methods.First(_ => _.Name == "Equals");

        var actionDefinition = FindTypeDefinition("System.Action");
        ActionTypeReference = ModuleDefinition.ImportReference(actionDefinition);

        var actionConstructor = actionDefinition.Methods.First(_ => _.IsConstructor);
        ActionConstructorReference = ModuleDefinition.ImportReference(actionConstructor);

        var propChangingInterfaceDefinition = FindTypeDefinition("System.ComponentModel.INotifyPropertyChanging");
        var propChangingHandlerDefinition = FindTypeDefinition("System.ComponentModel.PropertyChangingEventHandler");
        var propChangingArgsDefinition = FindTypeDefinition("System.ComponentModel.PropertyChangingEventArgs");


        PropChangingInterfaceReference = ModuleDefinition.ImportReference(propChangingInterfaceDefinition);
        PropChangingHandlerReference = ModuleDefinition.ImportReference(propChangingHandlerDefinition);
        ComponentModelPropertyChangingEventHandlerInvokeReference = ModuleDefinition.ImportReference(propChangingHandlerDefinition.Methods.First(_ => _.Name == "Invoke"));
        ComponentModelPropertyChangingEventConstructorReference = ModuleDefinition.ImportReference(propChangingArgsDefinition.Methods.First(_ => _.IsConstructor));

        var delegateDefinition = FindTypeDefinition("System.Delegate");
        var combineMethodDefinition = delegateDefinition.Methods
            .Single(_ =>
                _.Name == "Combine" &&
                _.Parameters.Count == 2 &&
                _.Parameters.All(_ => _.ParameterType == delegateDefinition));
        DelegateCombineMethodRef = ModuleDefinition.ImportReference(combineMethodDefinition);
        var removeMethodDefinition = delegateDefinition.Methods.First(_ => _.Name == "Remove");
        DelegateRemoveMethodRef = ModuleDefinition.ImportReference(removeMethodDefinition);

        var interlockedDefinition = FindTypeDefinition("System.Threading.Interlocked");
        var genericCompareExchangeMethodDefinition = interlockedDefinition
            .Methods.First(_ =>
                _.IsStatic &&
                _.Name == "CompareExchange" &&
                _.GenericParameters.Count == 1 &&
                _.Parameters.Count == 3);
        var genericCompareExchangeMethod = ModuleDefinition.ImportReference(genericCompareExchangeMethodDefinition);

        InterlockedCompareExchangeForPropChangingHandler = new(genericCompareExchangeMethod);
        InterlockedCompareExchangeForPropChangingHandler.GenericArguments.Add(PropChangingHandlerReference);
    }
}