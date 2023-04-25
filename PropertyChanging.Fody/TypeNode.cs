using System.Collections.Generic;
using Mono.Cecil;

public class TypeNode
{
    public TypeNode()
    {
        Nodes = new();
        PropertyDependencies = new();
        Mappings = new();
        PropertyDatas = new();
    }

    public TypeDefinition TypeDefinition;
    public List<TypeNode> Nodes;
    public List<PropertyDependency> PropertyDependencies;
    public List<MemberMapping> Mappings;
    public EventInvokerMethod EventInvoker;
    public MethodReference IsChangingInvoker;
    public List<PropertyData> PropertyDatas;
    public List<MethodReference> OnChangingMethods;
    public List<PropertyDefinition> AllProperties;
}