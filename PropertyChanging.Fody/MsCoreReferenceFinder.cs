using System;
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


    public void FindCoreReferences()
    {
        var assemblyResolver = ModuleDefinition.AssemblyResolver;
        var msCoreLibDefinition = assemblyResolver.Resolve("mscorlib");
        var msCoreTypes = msCoreLibDefinition.MainModule.Types;

        var objectDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Object");
        if (objectDefinition == null)
        {
            ExecuteWinRT();
            return;
        }
        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = ModuleDefinition.Import(constructorDefinition);


        var nullableDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Nullable");
        NullableEqualsMethod = ModuleDefinition.Import(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");

        var systemDefinition = assemblyResolver.Resolve("System");
        var systemTypes = systemDefinition.MainModule.Types;

        var actionDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Action");
        if (actionDefinition == null)
        {
            actionDefinition = systemTypes.FirstOrDefault(x => x.Name == "Action");
        }
        var systemCoreDefinition = GetSystemCoreDefinition();
        if (actionDefinition == null)
        {
            actionDefinition = systemCoreDefinition.MainModule.Types.First(x => x.Name == "Action");
        }
        ActionTypeReference = ModuleDefinition.Import(actionDefinition);

        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = ModuleDefinition.Import(actionConstructor);

		TypeDefinition propChangingHandlerDefinition;
		TypeDefinition propChangingArgsDefinition;
	    var propChangingInterfaceDefinition = systemTypes.FirstOrDefault(x => x.Name == "INotifyPropertyChanging");
	    if (propChangingInterfaceDefinition == null)
	    {
		    var mscorlibExtensionsDefinition = assemblyResolver.Resolve("mscorlib.Extensions");
			var mscorlibExtensionsTypes = mscorlibExtensionsDefinition.MainModule.Types;
			propChangingInterfaceDefinition = mscorlibExtensionsTypes.First(x => x.Name == "INotifyPropertyChanging");
		    propChangingHandlerDefinition = mscorlibExtensionsTypes.First(x => x.Name == "PropertyChangingEventHandler");
		    propChangingArgsDefinition = mscorlibExtensionsTypes.First(x => x.Name == "PropertyChangingEventArgs");
	    }
	    else
	    {
		    propChangingHandlerDefinition = systemTypes.First(x => x.Name == "PropertyChangingEventHandler");
		    propChangingArgsDefinition = systemTypes.First(x => x.Name == "PropertyChangingEventArgs");
	    }


	    PropChangingInterfaceReference = ModuleDefinition.Import(propChangingInterfaceDefinition);
        PropChangingHandlerReference = ModuleDefinition.Import(propChangingHandlerDefinition);
        ComponentModelPropertyChangingEventHandlerInvokeReference = ModuleDefinition.Import(propChangingHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        ComponentModelPropertyChangingEventConstructorReference = ModuleDefinition.Import(propChangingArgsDefinition.Methods.First(x => x.IsConstructor));

        var delegateDefinition = msCoreTypes.First(x => x.Name == "Delegate");
        var combineMethodDefinition = delegateDefinition.Methods
            .Single(x => 
                x.Name == "Combine" && 
                x.Parameters.Count == 2 && 
                x.Parameters.All(p => p.ParameterType == delegateDefinition));
        DelegateCombineMethodRef = ModuleDefinition.Import(combineMethodDefinition);
        var removeMethodDefinition = delegateDefinition.Methods.First(x => x.Name == "Remove");
        DelegateRemoveMethodRef = ModuleDefinition.Import(removeMethodDefinition);

        var interlockedDefinition = msCoreTypes.First(x => x.FullName == "System.Threading.Interlocked");
        var genericCompareExchangeMethodDefinition = interlockedDefinition
            .Methods.First(x => 
                x.IsStatic && 
                x.Name == "CompareExchange" && 
                x.GenericParameters.Count == 1 && 
                x.Parameters.Count == 3);
        var genericCompareExchangeMethod = ModuleDefinition.Import(genericCompareExchangeMethodDefinition);

        InterlockedCompareExchangeForPropChangingHandler = new GenericInstanceMethod(genericCompareExchangeMethod);
        InterlockedCompareExchangeForPropChangingHandler.GenericArguments.Add(PropChangingHandlerReference);
    }

    public void ExecuteWinRT()
    {
        var assemblyResolver = ModuleDefinition.AssemblyResolver;
        var systemRuntime = assemblyResolver.Resolve("System.Runtime");
        var systemRuntimeTypes = systemRuntime.MainModule.Types;

        var objectDefinition = systemRuntimeTypes.First(x => x.Name == "Object");

        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = ModuleDefinition.Import(constructorDefinition);


        var nullableDefinition = systemRuntimeTypes.FirstOrDefault(x => x.Name == "Nullable");
        NullableEqualsMethod = ModuleDefinition.Import(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");


        var actionDefinition = systemRuntimeTypes.First(x => x.Name == "Action");
        ActionTypeReference = ModuleDefinition.Import(actionDefinition);
        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = ModuleDefinition.Import(actionConstructor);


        var systemObjectModel = assemblyResolver.Resolve("System.ObjectModel");
        var systemObjectModelTypes = systemObjectModel.MainModule.Types;
        var propChangingHandlerDefinition = systemObjectModelTypes.First(x => x.Name == "PropertyChangingEventHandler");
        PropChangingHandlerReference = ModuleDefinition.Import(propChangingHandlerDefinition);
        ComponentModelPropertyChangingEventHandlerInvokeReference = ModuleDefinition.Import(propChangingHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        var propChangingArgsDefinition = systemObjectModelTypes.First(x => x.Name == "PropertyChangingEventArgs");
        ComponentModelPropertyChangingEventConstructorReference = ModuleDefinition.Import(propChangingArgsDefinition.Methods.First(x => x.IsConstructor));

        var delegateDefinition = systemRuntimeTypes.First(x => x.Name == "Delegate");
        var combineMethodDefinition = delegateDefinition.Methods
            .Single(x =>
                x.Name == "Combine" &&
                x.Parameters.Count == 2 &&
                x.Parameters.All(p => p.ParameterType == delegateDefinition));
        DelegateCombineMethodRef = ModuleDefinition.Import(combineMethodDefinition);
        var removeMethodDefinition = delegateDefinition.Methods.First(x => x.Name == "Remove");
        DelegateRemoveMethodRef = ModuleDefinition.Import(removeMethodDefinition);
        
        var systemThreading = assemblyResolver.Resolve("System.Threading");
        var interlockedDefinition = systemThreading.MainModule.Types.First(x => x.FullName == "System.Threading.Interlocked");
        var genericCompareExchangeMethodDefinition = interlockedDefinition
            .Methods.First(x =>
                x.IsStatic &&
                x.Name == "CompareExchange" &&
                x.GenericParameters.Count == 1 &&
                x.Parameters.Count == 3);
        var genericCompareExchangeMethod = ModuleDefinition.Import(genericCompareExchangeMethodDefinition);

        InterlockedCompareExchangeForPropChangingHandler = new GenericInstanceMethod(genericCompareExchangeMethod);
        InterlockedCompareExchangeForPropChangingHandler.GenericArguments.Add(PropChangingHandlerReference);
    }


    AssemblyDefinition GetSystemCoreDefinition()
    {
        try
        {
            return ModuleDefinition.AssemblyResolver.Resolve("System.Core");
        }
        catch (Exception exception)
        {
            var message = string.Format(@"Could not resolve System.Core. Please ensure you are using .net 3.5 or higher.{0}Inner message:{1}.", Environment.NewLine, exception.Message);
            throw new WeavingException(message);
        }
    }
}