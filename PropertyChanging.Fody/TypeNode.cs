using System.Collections.Generic;
using Mono.Cecil;

public class TypeNode
{
    public TypeNode()
    {
        Nodes = new List<TypeNode>();
        PropertyDependencies = new List<PropertyDependency>();
        Mappings = new List<MemberMapping>();
        PropertyDatas = new List<PropertyData>();
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