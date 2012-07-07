using System;
using System.Linq;
using Mono.Cecil;

public class MsCoreReferenceFinder
{
    ModuleWeaver moduleWeaver;
    IAssemblyResolver assemblyResolver;
    public MethodReference ComponentModelPropertyChangingEventHandlerInvokeReference;
    public MethodReference ComponentModelPropertyChangingEventConstructorReference;
    public MethodReference ActionConstructorReference;
    public MethodReference ObjectConstructor;
    public TypeReference ActionTypeReference;
    public MethodDefinition NullableEqualsMethod;
    public TypeReference PropChangingHandlerReference;

    public MsCoreReferenceFinder(ModuleWeaver moduleWeaver, IAssemblyResolver assemblyResolver)
    {
        this.moduleWeaver = moduleWeaver;
        this.assemblyResolver = assemblyResolver;
    }



    public void Execute()
    {
        var msCoreLibDefinition = assemblyResolver.Resolve("mscorlib");
        var msCoreTypes = msCoreLibDefinition.MainModule.Types;

        var objectDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Object");
        if (objectDefinition == null)
        {
            ExecuteWinRT();
            return;
        }
        var module = moduleWeaver.ModuleDefinition;
        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = module.Import(constructorDefinition);


        var nullableDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Nullable");
        NullableEqualsMethod = module.Import(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");

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
        ActionTypeReference = module.Import(actionDefinition);

        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = module.Import(actionConstructor);


        if (systemTypes.Any(x => x.Name == "PropertyChangingEventHandler"))
        {
            var propChangingHandlerDefinition = systemTypes.FirstOrDefault(x => x.Name == "PropertyChangingEventHandler");


            PropChangingHandlerReference = module.Import(propChangingHandlerDefinition);
            ComponentModelPropertyChangingEventHandlerInvokeReference = module.Import(propChangingHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
            var propChangingArgsDefinition = systemTypes.First(x => x.Name == "PropertyChangingEventArgs");
            ComponentModelPropertyChangingEventConstructorReference = module.Import(propChangingArgsDefinition.Methods.First(x => x.IsConstructor));
        }
        else
        {

            var mscorlibExtensions = assemblyResolver.Resolve("mscorlib.Extensions");
            var mscorlibExtensionsTypes = mscorlibExtensions.MainModule.Types;

            var propChangingHandlerDefinition = mscorlibExtensionsTypes.FirstOrDefault(x => x.Name == "PropertyChangingEventHandler");
            PropChangingHandlerReference = module.Import(propChangingHandlerDefinition);
            ComponentModelPropertyChangingEventHandlerInvokeReference = module.Import(propChangingHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
            var propChangingArgsDefinition = mscorlibExtensionsTypes.First(x => x.Name == "PropertyChangingEventArgs");
            ComponentModelPropertyChangingEventConstructorReference = module.Import(propChangingArgsDefinition.Methods.First(x => x.IsConstructor));
        }
    }

    public void ExecuteWinRT()
    {
        var systemRuntime = assemblyResolver.Resolve("System.Runtime");
        var systemRuntimeTypes = systemRuntime.MainModule.Types;

        var objectDefinition = systemRuntimeTypes.First(x => x.Name == "Object");

        var module = moduleWeaver.ModuleDefinition;
        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = module.Import(constructorDefinition);


        var nullableDefinition = systemRuntimeTypes.FirstOrDefault(x => x.Name == "Nullable");
        NullableEqualsMethod = module.Import(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");


        var actionDefinition = systemRuntimeTypes.First(x => x.Name == "Action");
        ActionTypeReference = module.Import(actionDefinition);
        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = module.Import(actionConstructor);

        var systemObjectModel = assemblyResolver.Resolve("System.ObjectModel");
        var systemObjectModelTypes = systemObjectModel.MainModule.Types;
        var propChangingHandlerDefinition = systemObjectModelTypes.First(x => x.Name == "PropertyChangingEventHandler");
        PropChangingHandlerReference = module.Import(propChangingHandlerDefinition);
        ComponentModelPropertyChangingEventHandlerInvokeReference = module.Import(propChangingHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        var propChangingArgsDefinition = systemObjectModelTypes.First(x => x.Name == "PropertyChangingEventArgs");
        ComponentModelPropertyChangingEventConstructorReference = module.Import(propChangingArgsDefinition.Methods.First(x => x.IsConstructor));

        var windowsRuntime = assemblyResolver.Resolve("System.Runtime.InteropServices.WindowsRuntime");
        var genericInstanceType = new GenericInstanceType(windowsRuntime.MainModule.Types.First(x => x.Name == "EventRegistrationTokenTable`1"));
        genericInstanceType.GenericArguments.Add(PropChangingHandlerReference);

    }


    AssemblyDefinition GetSystemCoreDefinition()
    {
        try
        {
            return assemblyResolver.Resolve("System.Core");
        }
        catch (Exception exception)
        {
            var message = string.Format(@"Could not resolve System.Core. Please ensure you are using .net 3.5 or higher.{0}Inner message:{1}.", Environment.NewLine, exception.Message);
            throw new WeavingException(message);
        }
    }
}