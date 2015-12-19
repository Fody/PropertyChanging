using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public class PropertyWeaver
{
    ModuleWeaver moduleWeaver;
    PropertyData propertyData;
    TypeNode typeNode;
    readonly TypeSystem typeSystem;
    MethodBody setMethodBody;
    Collection<Instruction> instructions;

    public PropertyWeaver(ModuleWeaver moduleWeaver, PropertyData propertyData, TypeNode typeNode, TypeSystem typeSystem)
    {
        this.moduleWeaver = moduleWeaver;
        this.propertyData = propertyData;
        this.typeNode = typeNode;
        this.typeSystem = typeSystem;
    }

    public void Execute()
    {
        moduleWeaver.LogInfo("\t\t" + propertyData.PropertyDefinition.Name);
        var property = propertyData.PropertyDefinition;
        setMethodBody = property.SetMethod.Body;
        instructions = property.SetMethod.Body.Instructions;

        foreach (var instruction in GetInstructions())
        {
            Inject(instruction);
        }
    }

    List<Instruction> GetInstructions()
    {
        if (propertyData.BackingFieldReference == null)
        {
            return new List<Instruction> { instructions.First() };
        }
        var setFieldInstructions = FindSetFieldInstructions().ToList();
        if (setFieldInstructions.Count == 0)
        {
            return new List<Instruction> { instructions.First() };
        }

        return setFieldInstructions;
    }

    void Inject(Instruction instruction)
    {
        InjectNop(instruction);
        var index = instructions.IndexOf(instruction);
        var propertyDefinitions = propertyData.AlsoNotifyFor.Distinct();

        index = propertyDefinitions.Aggregate(index, AddEventInvokeCall);
        AddEventInvokeCall(index, propertyData.PropertyDefinition);
    }

    void InjectNop(Instruction instruction)
    {
        Instruction nop;
        if (instruction.Previous != null && instruction.Previous.OpCode == OpCodes.Nop)
        {
            nop = instruction.Previous;
        }
        else
        {
            nop = Instruction.Create(OpCodes.Nop);
            instructions.Insert(instructions.IndexOf(instruction), nop);
        }
        foreach (var innerInstruction in instructions)
        {
            if (innerInstruction == instruction)
            {
                continue;
            }
            var flowControl = innerInstruction.OpCode.FlowControl;
            if (flowControl != FlowControl.Branch && flowControl != FlowControl.Cond_Branch)
            {
                continue;
            }
            if (innerInstruction.Operand != instruction)
            {
                continue;
            }
            innerInstruction.Operand = nop;
        }
    }


    IEnumerable<Instruction> FindSetFieldInstructions()
    {
        return from instruction in instructions
               where instruction.OpCode == OpCodes.Stfld
               let fieldReference = instruction.Operand as FieldReference
               where fieldReference != null
               where fieldReference.Name == propertyData.BackingFieldReference.Name
               select instruction.Previous.Previous;
    }


    int AddEventInvokeCall(int index, PropertyDefinition property)
    {
        index = AddOnChangingMethodCall(index, property);
        if (propertyData.AlreadyNotifies.Contains(property.Name))
        {
            moduleWeaver.LogInfo($"\t\t\t{property.Name} skipped since call already exists");
            return index;
        }
        moduleWeaver.LogInfo($"\t\t\t{property.Name}");
        if (typeNode.EventInvoker.InvokerType == InvokerTypes.Before)
        {
            return AddBeforeInvokerCall(index, property);
        }
        if (typeNode.EventInvoker.InvokerType == InvokerTypes.PropertyChangingArg)
        {
            return AddPropertyChangedArgInvokerCall(index, property);
        }
        return AddSimpleInvokerCall(index, property);
    }

    int AddOnChangingMethodCall(int index, PropertyDefinition property)
    {
        if (!moduleWeaver.InjectOnPropertyNameChanging)
        {
            return index;
        }
        var onChangingMethodName = $"On{property.Name}Changing";
        if (ContainsCallToMethod(onChangingMethodName))
        {
            return index;
        }
        var onChangingMethod = typeNode
            .OnChangingMethods
            .FirstOrDefault(x => x.Name == onChangingMethodName);
        if (onChangingMethod == null)
        {
            return index;
        }
        return instructions.Insert(index,Instruction.Create(OpCodes.Ldarg_0),CreateCall(onChangingMethod));
    }

    bool ContainsCallToMethod(string onChangingMethodName)
    {
        return instructions.Select(x => x.Operand)
            .OfType<MethodReference>()
            .Any(x => x.Name == onChangingMethodName);
    }

    int AddSimpleInvokerCall(int index, PropertyDefinition property)
    {
        return instructions.Insert(index,
                                   Instruction.Create(OpCodes.Ldarg_0),
                                   Instruction.Create(OpCodes.Ldstr, property.Name),
                                   CallEventInvoker());
    }

    int AddPropertyChangedArgInvokerCall(int index, PropertyDefinition property)
    {
        return instructions.Insert(index,
                                   Instruction.Create(OpCodes.Ldarg_0),
                                   Instruction.Create(OpCodes.Ldstr, property.Name),
                                   Instruction.Create(OpCodes.Newobj, moduleWeaver.ComponentModelPropertyChangingEventConstructorReference),
                                   CallEventInvoker());
    }

    int AddBeforeInvokerCall(int index, PropertyDefinition property)
    {
        var beforeVariable = new VariableDefinition(typeSystem.Object);
        setMethodBody.Variables.Add(beforeVariable);
        var getMethod = property.GetMethod.GetGeneric();

        return instructions.Insert(index,
                                   Instruction.Create(OpCodes.Ldarg_0),
                                   CreateCall(getMethod),
                                   Instruction.Create(OpCodes.Box, property.GetMethod.ReturnType),
                                   Instruction.Create(OpCodes.Stloc, beforeVariable),
                                   Instruction.Create(OpCodes.Ldarg_0),
                                   Instruction.Create(OpCodes.Ldstr, property.Name),
                                   Instruction.Create(OpCodes.Ldloc, beforeVariable),
                                   CallEventInvoker());
    }

    public Instruction CallEventInvoker()
    {
        return Instruction.Create(OpCodes.Callvirt, typeNode.EventInvoker.MethodReference);
    }

    public Instruction CreateCall(MethodReference methodReference)
    {
        return Instruction.Create(OpCodes.Callvirt, methodReference);
    }

}