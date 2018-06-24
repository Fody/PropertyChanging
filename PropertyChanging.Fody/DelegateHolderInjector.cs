using Mono.Cecil;
using Mono.Cecil.Cil;

public class DelegateHolderInjector
{
    public TypeDefinition TargetTypeDefinition;
    public MethodReference OnPropertyChangingMethodReference ;
    public ModuleWeaver ModuleWeaver;

    public void InjectDelegateHolder()
    {
        var attributes = TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.NestedPrivate | TypeAttributes.BeforeFieldInit;
        TypeDefinition = new TypeDefinition(null, "<>PropertyNotificationDelegateHolder", attributes, ModuleWeaver.TypeSystem.ObjectReference);
        CreateFields(TargetTypeDefinition);
        CreateOnPropChanging(OnPropertyChangingMethodReference);
        CreateConstructor();
        TargetTypeDefinition.NestedTypes.Add(TypeDefinition);
    }

    void CreateFields(TypeDefinition targetTypeDefinition)
    {
        Target = new FieldDefinition("target", FieldAttributes.Public, targetTypeDefinition);
        TypeDefinition.Fields.Add(Target);
        PropertyName = new FieldDefinition("propertyName", FieldAttributes.Public, ModuleWeaver.TypeSystem.StringReference);
        TypeDefinition.Fields.Add(PropertyName);
    }

    void CreateOnPropChanging(MethodReference onPropertyChangingMethodReference)
    {
        var attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
        MethodDefinition = new MethodDefinition("OnPropertyChanging", attributes, ModuleWeaver.TypeSystem.VoidReference);
        MethodDefinition.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, Target),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, PropertyName),
            Instruction.Create(OpCodes.Callvirt, onPropertyChangingMethodReference),
            Instruction.Create(OpCodes.Ret)
            );
        TypeDefinition.Methods.Add(MethodDefinition);
    }

    void CreateConstructor()
    {
        var attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        ConstructorDefinition = new MethodDefinition(".ctor", attributes, ModuleWeaver.TypeSystem.VoidReference);
        ConstructorDefinition.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Call, ModuleWeaver.ObjectConstructor),
            Instruction.Create(OpCodes.Ret)
            );
        TypeDefinition.Methods.Add(ConstructorDefinition);
    }

    public MethodDefinition MethodDefinition;
    public FieldDefinition PropertyName;
    public FieldDefinition Target;
    public TypeDefinition TypeDefinition;
    public MethodDefinition ConstructorDefinition;
}