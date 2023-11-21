using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void ProcessOnChangingMethods()
    {
        ProcessOnChangingMethods(NotifyNodes);
    }

    void ProcessOnChangingMethods(List<TypeNode> notifyNodes)
    {
        foreach (var notifyNode in notifyNodes)
        {
            notifyNode.OnChangingMethods = GetOnChangingMethods(notifyNode).ToList();
            ProcessOnChangingMethods(notifyNode.Nodes);
        }
    }

    IEnumerable<MethodReference> GetOnChangingMethods(TypeNode notifyNode)
    {
        var methods = notifyNode.TypeDefinition.Methods;

        return methods.Where(_ => !_.IsStatic &&
                                  !_.IsAbstract &&
                                  _.Parameters.Count == 0 &&
                                  _.Name.StartsWith("On") &&
                                  _.Name.EndsWith("Changing"))
            .Select(methodDefinition =>
            {
                var typeDefinitions = new Stack<TypeDefinition>();
                typeDefinitions.Push(notifyNode.TypeDefinition);

                return GetMethodReference(typeDefinitions, methodDefinition);
            });
    }
}