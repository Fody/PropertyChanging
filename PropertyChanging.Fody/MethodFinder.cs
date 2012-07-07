﻿using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class MethodFinder
{
    MethodGenerifier methodGenerifier;
    MethodInjector methodInjector;
    TypeNodeBuilder typeNodeBuilder;
    ModuleWeaver moduleWeaver;
    TypeResolver typeResolver;
    EventInvokerNameResolver eventInvokerNameResolver;

    public MethodFinder(MethodGenerifier methodGenerifier, MethodInjector methodInjector, TypeNodeBuilder typeNodeBuilder, ModuleWeaver moduleWeaver, TypeResolver typeResolver, EventInvokerNameResolver eventInvokerNameResolver)
    {
        this.methodGenerifier = methodGenerifier;
        this.methodInjector = methodInjector;
        this.typeNodeBuilder = typeNodeBuilder;
        this.moduleWeaver = moduleWeaver;
        this.typeResolver = typeResolver;
        this.eventInvokerNameResolver = eventInvokerNameResolver;
    }


    void ProcessChildNode(TypeNode node, EventInvokerMethod eventInvoker)
    {
        var childEventInvoker = FindEventInvokerMethod(node.TypeDefinition);
        if (childEventInvoker == null)
        {
            if (node.TypeDefinition.BaseType.IsGenericInstance)
            {
                var methodReference = MethodGenerifier.MakeGeneric(node.TypeDefinition.BaseType, eventInvoker.MethodReference);
                eventInvoker = new EventInvokerMethod
                                   {
                                       IsBefore = eventInvoker.IsBefore,
                                       MethodReference = methodReference,
                                   };
            }
        }
        else
        {
            eventInvoker = childEventInvoker;
        }

        node.EventInvoker = eventInvoker;

        foreach (var childNode in node.Nodes)
        {
            ProcessChildNode(childNode, eventInvoker);
        }
    }



    public EventInvokerMethod RecursiveFindMethod(TypeDefinition typeDefinition)
    {
        var typeDefinitions = new Stack<TypeDefinition>();
        MethodDefinition methodDefinition;
        var currentTypeDefinition = typeDefinition;
        do
        {
            typeDefinitions.Push(currentTypeDefinition);
            if (FindEventInvokerMethodDefinition(currentTypeDefinition, out methodDefinition))
            {
                break;
            }
            var baseType = currentTypeDefinition.BaseType;

            if (baseType == null || baseType.FullName == "System.Object")
            {
                return null;
            }
            currentTypeDefinition = typeResolver.Resolve(baseType);
        } while (true);

        return new EventInvokerMethod
                   {
                       MethodReference = methodGenerifier.GetMethodReference(typeDefinitions, methodDefinition),
                       IsBefore = IsBeforeMethod(methodDefinition),
                   };
    }


    EventInvokerMethod FindEventInvokerMethod(TypeDefinition type)
    {
        MethodDefinition methodDefinition;
        if (FindEventInvokerMethodDefinition(type, out methodDefinition))
        {
            var methodReference = moduleWeaver.ModuleDefinition.Import(methodDefinition);
            return new EventInvokerMethod
                       {
                           MethodReference = methodReference.GetGeneric(),
                           IsBefore = IsBeforeMethod(methodDefinition),
                       };
        }
        return null;
    }

    bool FindEventInvokerMethodDefinition(TypeDefinition type, out MethodDefinition methodDefinition)
    {
        methodDefinition = type.Methods
            .Where(x => eventInvokerNameResolver.EventInvokerNames.Contains(x.Name))
            .OrderByDescending(definition => definition.Parameters.Count)
            .FirstOrDefault(x => IsBeforeMethod(x) || IsSingleStringMethod(x));
        return methodDefinition != null;
    }

    public static bool IsSingleStringMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 1
               && parameters[0].ParameterType.FullName == "System.String";
    }

    public static bool IsBeforeMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 2
               && parameters[0].ParameterType.FullName == "System.String"
               && parameters[1].ParameterType.FullName == "System.Object";
    }


    public void Execute()
    {
        foreach (var notifyNode in typeNodeBuilder.NotifyNodes)
        {
            var eventInvoker = RecursiveFindMethod(notifyNode.TypeDefinition);
            if (eventInvoker == null)
            {
                eventInvoker = methodInjector.AddOnPropertyChangingMethod(notifyNode.TypeDefinition);
                if (eventInvoker == null)
                {
                    var message = string.Format("\tCould not derive or inject '{0}' into '{1}'. It is possible you are inheriting from a base class and have not correctly set 'EventInvokerNames' or you are using a explicit PropertyChanging event and the event field is not visible to this instance. Please either correct 'EventInvokerNames' or implement your own EventInvoker on this class. No derived types will be processed. If you want to supress this message place a [DoNotNotifyAttribute] on {1}.", string.Join(", ", eventInvokerNameResolver.EventInvokerNames), notifyNode.TypeDefinition.Name);
                    moduleWeaver.LogWarning(message);
                    continue;
                }
            }

            notifyNode.EventInvoker = eventInvoker;

            foreach (var childNode in notifyNode.Nodes)
            {
                ProcessChildNode(childNode, eventInvoker);
            }
        }
    }

}