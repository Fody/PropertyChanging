using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
    public void InjectINotifyPropertyChangingInterface(TypeDefinition targetType)
    {
        targetType.Interfaces.Add(new(PropChangingInterfaceReference));
        WeaveEvent(targetType);
    }

    // Thank you to Romain Verdier
    // largely copied from http://codingly.com/2008/11/10/introduction-a-monocecil-implementer-inotifypropertychanging/
    void WeaveEvent(TypeDefinition type)
    {
        var propertyChangingFieldDef = new FieldDefinition("PropertyChanging", FieldAttributes.Private, PropChangingHandlerReference);
        type.Fields.Add(propertyChangingFieldDef);
        var propertyChangingField = propertyChangingFieldDef.GetGeneric();
        var eventDefinition = new EventDefinition("PropertyChanging", EventAttributes.None, PropChangingHandlerReference)
                              {
                                  AddMethod = CreateEventMethod("add_PropertyChanging", DelegateCombineMethodRef, propertyChangingField),
                                  RemoveMethod = CreateEventMethod("remove_PropertyChanging", DelegateRemoveMethodRef, propertyChangingField)
                              };

        type.Methods.Add(eventDefinition.AddMethod);
        type.Methods.Add(eventDefinition.RemoveMethod);
        type.Events.Add(eventDefinition);
    }

    MethodDefinition CreateEventMethod(string methodName, MethodReference delegateMethodReference, FieldReference propertyChangingField)
    {
        const MethodAttributes Attributes = MethodAttributes.Public |
            MethodAttributes.HideBySig |
            MethodAttributes.Final |
            MethodAttributes.SpecialName |
            MethodAttributes.NewSlot |
            MethodAttributes.Virtual;

        var method = new MethodDefinition(methodName, Attributes, TypeSystem.VoidReference);

        method.Parameters.Add(new("value", ParameterAttributes.None, PropChangingHandlerReference));
        var handlerVariable0 = new VariableDefinition(PropChangingHandlerReference);
        method.Body.Variables.Add(handlerVariable0);
        var handlerVariable1 = new VariableDefinition(PropChangingHandlerReference);
        method.Body.Variables.Add(handlerVariable1);
        var handlerVariable2 = new VariableDefinition(PropChangingHandlerReference);
        method.Body.Variables.Add(handlerVariable2);

        var loopBegin = Instruction.Create(OpCodes.Ldloc, handlerVariable0);
        method.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, propertyChangingField),
            Instruction.Create(OpCodes.Stloc, handlerVariable0),
            loopBegin,
            Instruction.Create(OpCodes.Stloc, handlerVariable1),
            Instruction.Create(OpCodes.Ldloc, handlerVariable1),
            Instruction.Create(OpCodes.Ldarg_1),
            Instruction.Create(OpCodes.Call, delegateMethodReference),
            Instruction.Create(OpCodes.Castclass, PropChangingHandlerReference),
            Instruction.Create(OpCodes.Stloc, handlerVariable2),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldflda, propertyChangingField),
            Instruction.Create(OpCodes.Ldloc, handlerVariable2),
            Instruction.Create(OpCodes.Ldloc, handlerVariable1),
            Instruction.Create(OpCodes.Call, InterlockedCompareExchangeForPropChangingHandler),
            Instruction.Create(OpCodes.Stloc, handlerVariable0),
            Instruction.Create(OpCodes.Ldloc, handlerVariable0),
            Instruction.Create(OpCodes.Ldloc, handlerVariable1),
            Instruction.Create(OpCodes.Bne_Un_S, loopBegin), // goto begin of loop
            Instruction.Create(OpCodes.Ret));
        method.Body.InitLocals = true;

        return method;
    }
}