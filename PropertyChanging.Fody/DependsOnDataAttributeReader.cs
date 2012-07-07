﻿using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class DependsOnDataAttributeReader
{
    TypeNodeBuilder typeNodeBuilder;
    ModuleWeaver moduleWeaver;

    public DependsOnDataAttributeReader(TypeNodeBuilder typeNodeBuilder, ModuleWeaver moduleWeaver)
    {
        this.typeNodeBuilder = typeNodeBuilder;
        this.moduleWeaver = moduleWeaver;
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            Process(node);
            Process(node.Nodes);
        }
    }

    public void Process(TypeNode node)
    {
        foreach (var propertyDefinition in node.TypeDefinition.Properties)
        {
            Read(propertyDefinition, node);
        }
    }

    public void Read(PropertyDefinition property, TypeNode node)
    {
        var dependsOnAttribute = property.CustomAttributes.GetAttribute("PropertyChanging.DependsOnAttribute");
        if (dependsOnAttribute == null)
        {
            return;
        }
        var customAttributeArguments = dependsOnAttribute.ConstructorArguments.ToList();
        var value = (string) customAttributeArguments[0].Value;
        AddIfPropertyExists(property, value, node);
        if (customAttributeArguments.Count > 1)
        {
            var otherValue = (CustomAttributeArgument[]) customAttributeArguments[1].Value;
            foreach (string other in otherValue.Select(x => x.Value))
            {
                AddIfPropertyExists(property, other, node);
            }
        }
    }

    void AddIfPropertyExists(PropertyDefinition targetProperty, string isGeneratedUsingPropertyName, TypeNode node)
    {
        //TODO: all properties
        var propertyDefinition = targetProperty.DeclaringType.Properties.FirstOrDefault(x => x.Name == isGeneratedUsingPropertyName);
        if (propertyDefinition == null)
        {
            moduleWeaver.LogInfo(string.Format("Could not find property '{0}' for DependsOnAttribute assigged to '{1}'.", isGeneratedUsingPropertyName, targetProperty.Name));
            return;
        }
        node.PropertyDependencies.Add(new PropertyDependency
                                          {
                                              WhenPropertyIsSet = propertyDefinition,
                                              ShouldAlsoNotifyFor = targetProperty
                                          });
    }


    public void Execute()
    {
        Process(typeNodeBuilder.NotifyNodes);
    }
}