﻿using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

public partial class ModuleWeaver
{

    void WalkPropertyData(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            foreach (var property in node.TypeDefinition.Properties)
            {
                if (property.CustomAttributes.ContainsAttribute("PropertyChanging.DoNotNotifyAttribute"))
                {
                    continue;
                }

                if (property.SetMethod == null)
                {
                    continue;
                }

                if (property.SetMethod.IsStatic)
                {
                    continue;
                }

                GetPropertyData(property, node);
            }

            WalkPropertyData(node.Nodes);
        }
    }

    //if (AlreadyContainsNotification(propertyData.PropertyDefinition, node.EventInvoker.MethodReference.Name))
    //          {
    //              moduleWeaver.LogInfo(string.Format("\t{0} Already has notification functionality. Property will be ignored.", propertyData.PropertyDefinition.GetName()));
    //              continue;
    //          }
    static void GetPropertyData(PropertyDefinition propertyDefinition, TypeNode node)
    {
        var notifyPropertyData = ReadAlsoNotifyForData(propertyDefinition, node.AllProperties);
        var dependenciesForProperty = node.PropertyDependencies
            .Where(x => x.WhenPropertyIsSet == propertyDefinition)
            .Select(x => x.ShouldAlsoNotifyFor);

        var backingFieldReference = node.Mappings.First(x => x.PropertyDefinition == propertyDefinition).FieldDefinition;
        if (notifyPropertyData == null)
        {
            if (node.EventInvoker == null)
            {
                return;
            }

            node.PropertyDatas.Add(new()
            {
                BackingFieldReference = backingFieldReference,
                PropertyDefinition = propertyDefinition,
                // Compute full dependencies for the current property
                AlsoNotifyFor = GetFullDependencies(propertyDefinition, dependenciesForProperty, node),
                AlreadyNotifies = propertyDefinition.GetAlreadyNotifies(node.EventInvoker.MethodReference.Name).ToList()
            });
            return;
        }

        if (node.EventInvoker == null)
        {
            throw new WeavingException($@"Could not find field for PropertyChanging event on type '{node.TypeDefinition.FullName}'.
Looked for 'PropertyChanging', 'propertyChanging', '_PropertyChanging' and '_propertyChanging'.
The most likely cause is that you have implemented a custom event accessor for the PropertyChanging event and have called the PropertyChangingEventHandler something stupid.");
        }

        node.PropertyDatas.Add(new()
        {
            BackingFieldReference = backingFieldReference,
            PropertyDefinition = propertyDefinition,
            // Compute full dependencies for the current property
            AlsoNotifyFor = GetFullDependencies(propertyDefinition, notifyPropertyData.AlsoNotifyFor.Union(dependenciesForProperty), node),
            AlreadyNotifies = propertyDefinition.GetAlreadyNotifies(node.EventInvoker.MethodReference.Name).ToList()
        });
    }

    static List<PropertyDefinition> GetFullDependencies(PropertyDefinition propertyDefinition, IEnumerable<PropertyDefinition> dependenciesForProperty, TypeNode node)
    {
        // Create an HashSet to contain all dependent properties (direct or transitive)
        // Add the initial Property to stop the recursion if this property is a dependency of another property
        var fullDependencies = new HashSet<PropertyDefinition> {propertyDefinition};

        foreach (var dependentProperty in dependenciesForProperty)
        {
            // Check if the property is already present in the HashSet before starting the recursion
            if (fullDependencies.Add(dependentProperty))
            {
                ComputeDependenciesRec(dependentProperty, fullDependencies, node);
            }
        }

        // Remove the initial Property of the HashSet.
        fullDependencies.Remove(propertyDefinition);

        return fullDependencies.ToList();
    }

    /// <summary>
    /// Computes dependencies recursively
    /// </summary>
    static void ComputeDependenciesRec(PropertyDefinition propertyDefinition, HashSet<PropertyDefinition> fullDependencies, TypeNode node)
    {
        // TODO: An optimization could be done to avoid the multiple computation of one property for each property of the type
        // By keeping the in memory the full dependencies of each property of the type

        foreach (var dependentProperty in node.PropertyDependencies.Where(x => x.WhenPropertyIsSet == propertyDefinition).Select(x => x.ShouldAlsoNotifyFor))
        {
            if (fullDependencies.Contains(dependentProperty))
            {
                continue;
            }

            fullDependencies.Add(dependentProperty);

            ComputeDependenciesRec(dependentProperty, fullDependencies, node);
        }
    }

    public void WalkPropertyData()
    {
        WalkPropertyData(NotifyNodes);
    }
}