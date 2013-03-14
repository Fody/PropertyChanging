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

        return methods.Where(x => !x.IsStatic &&
                                  !x.IsAbstract &&
                                  x.Parameters.Count == 0 &&
                                  x.Name.StartsWith("On") &&
                                  x.Name.EndsWith("Changing"))
            .Select(methodDefinition =>
            {
                var typeDefinitions = new Stack<TypeDefinition>();
                typeDefinitions.Push(notifyNode.TypeDefinition);

                return GetMethodReference(typeDefinitions, methodDefinition);
            });
    }
}