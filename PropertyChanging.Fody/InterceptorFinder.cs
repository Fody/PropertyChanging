﻿using System.Linq;
using Fody;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public bool FoundInterceptor;
    public MethodDefinition InterceptMethod;
    public InvokerTypes InterceptorType;

    void SearchForMethod(TypeDefinition typeDefinition)
    {
        var methodDefinition = typeDefinition.Methods.FirstOrDefault(_ => _.Name == "Intercept");
        if (methodDefinition == null)
        {
            throw new WeavingException($"Found Type '{typeDefinition.FullName}' but could not find a method named 'Intercept'.");
        }
        if (!methodDefinition.IsStatic)
        {
            throw new WeavingException($"Found Type '{typeDefinition.FullName}.Intercept' but it is not static.");
        }
        if (!methodDefinition.IsPublic)
        {
            throw new WeavingException($"Found Type '{typeDefinition.FullName}.Intercept' but it is not public.");
        }

        if (IsSingleStringInterceptionMethod(methodDefinition))
        {
            FoundInterceptor = true;
            InterceptMethod = methodDefinition;
            InterceptorType = InvokerTypes.String;
            return;
        }
        if (IsBeforeInterceptionMethod(methodDefinition))
        {
            FoundInterceptor = true;
            InterceptMethod = methodDefinition;
            InterceptorType = InvokerTypes.Before;
            return;
        }
        var message = $@"Found '{typeDefinition.FullName}.Intercept' But the signature is not correct. It needs to be either.
Intercept(object target, Action firePropertyChanging, string propertyName)
or
Intercept(object target, Action firePropertyChanging, string propertyName, object before)";
        throw new WeavingException(message);
    }


    public bool IsSingleStringInterceptionMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 3
               && parameters[0].ParameterType.FullName == "System.Object"
               && parameters[1].ParameterType.FullName == "System.Action"
               && parameters[2].ParameterType.FullName == "System.String";
    }

    public bool IsBeforeInterceptionMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 4
               && parameters[0].ParameterType.FullName == "System.Object"
               && parameters[1].ParameterType.FullName == "System.Action"
               && parameters[2].ParameterType.FullName == "System.String"
               && parameters[3].ParameterType.FullName == "System.Object";
    }

    public void FindInterceptor()
    {
        var typeDefinition = ModuleDefinition.Types.FirstOrDefault(_ => _.Name == "PropertyChangingNotificationInterceptor");
        if (typeDefinition == null)
        {
            FoundInterceptor = false;
            return;
        }
        SearchForMethod(typeDefinition);
    }
}