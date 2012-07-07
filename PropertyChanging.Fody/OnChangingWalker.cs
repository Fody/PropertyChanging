﻿using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class OnChangingWalker
{
    MethodGenerifier methodGenerifier;
    TypeNodeBuilder typeNodeBuilder;

    public OnChangingWalker(MethodGenerifier methodGenerifier, TypeNodeBuilder typeNodeBuilder)
    {
        this.methodGenerifier = methodGenerifier;
        this.typeNodeBuilder = typeNodeBuilder;
    }

    public void Execute()
    {
        Process(typeNodeBuilder.NotifyNodes);
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var notifyNode in notifyNodes)
        {
            notifyNode.OnChangingMethods = GetOnChangingMethods(notifyNode).ToList();
            Process(notifyNode.Nodes);
        }
    }

  
    IEnumerable<MethodReference> GetOnChangingMethods(TypeNode notifyNode)
    {
        var methods = notifyNode.TypeDefinition.Methods;

        return methods.Where(x => !x.IsStatic &&
                                  !x.IsAbstract &&
                                  x.Parameters.Count == 0 &&
                                  x.Name.StartsWith("On") &&
                                  x.Name.EndsWith("Changing"))
            .Select(methodDefinition =>
            {
                var typeDefinitions = new Stack<TypeDefinition>();
                typeDefinitions.Push(notifyNode.TypeDefinition);

                return methodGenerifier.GetMethodReference(typeDefinitions, methodDefinition);
            });
    }
}