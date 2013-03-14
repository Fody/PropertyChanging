using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{

    public EventInvokerMethod AddOnPropertyChangingMethod(TypeDefinition targetType)
    {
        var propertyChangingField = FindPropertyChangingField(targetType);
        if (propertyChangingField == null)
        {
            return null;
        }
        if (FoundInterceptor)
        {
            var methodDefinition = GetMethodDefinition(targetType, propertyChangingField);

            return new EventInvokerMethod
                       {
                           MethodReference = InjectInterceptedMethod(targetType, methodDefinition).GetGeneric(),
                           IsBefore = IsInterceptorBefore,
                       };
        }
        return new EventInvokerMethod
                   {
                       MethodReference = InjectMethod(targetType, EventInvokerNames.First(), propertyChangingField).GetGeneric(),
                       IsBefore= false,
                   };
    }

    MethodDefinition GetMethodDefinition(TypeDefinition targetType, FieldReference propertyChangingField)
    {
        var eventInvokerName = "Inner" + EventInvokerNames.First();
        var methodDefinition = targetType.Methods.FirstOrDefault(x => x.Name == eventInvokerName);
        if (methodDefinition != null)
        {
            if (methodDefinition.Parameters.Count == 1 && methodDefinition.Parameters[0].ParameterType.FullName == "System.String")
            {
                return methodDefinition;
            }
        }
        return InjectMethod(targetType, eventInvokerName, propertyChangingField);
    }

    MethodDefinition InjectMethod(TypeDefinition targetType, string eventInvokerName, FieldReference propertyChangingField)
    {
        var method = new MethodDefinition(eventInvokerName, GetMethodAttributes(targetType), ModuleDefinition.TypeSystem.Void);
		method.Parameters.Add(new ParameterDefinition("propertyName", ParameterAttributes.None, ModuleDefinition.TypeSystem.String));

        var handlerVariable = new VariableDefinition(PropChangingHandlerReference);
        method.Body.Variables.Add(handlerVariable);
		var boolVariable = new VariableDefinition(ModuleDefinition.TypeSystem.Boolean);
        method.Body.Variables.Add(boolVariable);

        var instructions = method.Body.Instructions;

        var last = Instruction.Create(OpCodes.Ret);
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldfld, propertyChangingField)); 
        instructions.Add(Instruction.Create(OpCodes.Stloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldnull));
        instructions.Add(Instruction.Create(OpCodes.Ceq));
        instructions.Add(Instruction.Create(OpCodes.Stloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Brtrue_S, last));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        instructions.Add(Instruction.Create(OpCodes.Newobj, ComponentModelPropertyChangingEventConstructorReference));
        instructions.Add(Instruction.Create(OpCodes.Callvirt, ComponentModelPropertyChangingEventHandlerInvokeReference));


        instructions.Add(last);
        method.Body.InitLocals = true;
        targetType.Methods.Add(method);
        return method;
    }

    static MethodAttributes GetMethodAttributes(TypeDefinition targetType)
    {
        if (targetType.IsSealed)
        {
            return MethodAttributes.Public | MethodAttributes.HideBySig;
        }
        return MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.NewSlot;
    }

    public static FieldReference FindPropertyChangingField(TypeDefinition targetType)
    {
        var findPropertyChangingField = targetType.Fields.FirstOrDefault(x => IsPropertyChangingEventHandler(x.FieldType));
        if (findPropertyChangingField == null)
        {
            return null;
        }
        return findPropertyChangingField.GetGeneric();
    }

    MethodDefinition InjectInterceptedMethod(TypeDefinition targetType, MethodDefinition innerOnPropertyChanging)
    {
		InjectDelegateHolder(targetType, innerOnPropertyChanging);
		var method = new MethodDefinition(EventInvokerNames.First(), GetMethodAttributes(targetType), ModuleDefinition.TypeSystem.Void);

		var propertyName = new ParameterDefinition("propertyName", ParameterAttributes.None, ModuleDefinition.TypeSystem.String);
        method.Parameters.Add(propertyName);
        if (IsInterceptorBefore)
        {
			var before = new ParameterDefinition("before", ParameterAttributes.None, ModuleDefinition.TypeSystem.Object);
            method.Parameters.Add(before);
        }

        var action = new VariableDefinition("firePropertyChanging", ActionTypeReference);
        method.Body.Variables.Add(action);

        var variableDefinition = new VariableDefinition("delegateHolder", TypeDefinition);
        method.Body.Variables.Add(variableDefinition);


        var instructions = method.Body.Instructions;

        var last = Instruction.Create(OpCodes.Ret);
        instructions.Add(Instruction.Create(OpCodes.Newobj, ConstructorDefinition));
        instructions.Add(Instruction.Create(OpCodes.Stloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        instructions.Add(Instruction.Create(OpCodes.Stfld, PropertyName));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Stfld, Target));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldftn, MethodDefinition));
        instructions.Add(Instruction.Create(OpCodes.Newobj, ActionConstructorReference));
        instructions.Add(Instruction.Create(OpCodes.Stloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldfld, PropertyName));
        if (IsInterceptorBefore)
        {
            instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            instructions.Add(Instruction.Create(OpCodes.Call, InterceptMethod));
        }
        else
        {
            instructions.Add(Instruction.Create(OpCodes.Call, InterceptMethod));
        }

        instructions.Add(last);
        method.Body.InitLocals = true;

        targetType.Methods.Add(method);
        return method;
    }
}