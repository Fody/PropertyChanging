using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class MethodInjector
{
    InterceptorFinder interceptorFinder;
    DelegateHolderInjector delegateHolderInjector;
    MsCoreReferenceFinder msCoreReferenceFinder;
    EventInvokerNameResolver eventInvokerNameResolver;
    readonly TypeSystem typeSystem;

    public MethodInjector(InterceptorFinder interceptorFinder, DelegateHolderInjector delegateHolderInjector, MsCoreReferenceFinder msCoreReferenceFinder, EventInvokerNameResolver eventInvokerNameResolver, TypeSystem typeSystem)
    {
        this.interceptorFinder = interceptorFinder;
        this.delegateHolderInjector = delegateHolderInjector;
        this.msCoreReferenceFinder = msCoreReferenceFinder;
        this.eventInvokerNameResolver = eventInvokerNameResolver;
        this.typeSystem = typeSystem;
    }

    public EventInvokerMethod AddOnPropertyChangingMethod(TypeDefinition targetType)
    {
        var propertyChangingField = FindPropertyChangingField(targetType);
        if (propertyChangingField == null)
        {
            return null;
        }
        if (interceptorFinder.Found)
        {
            var methodDefinition = GetMethodDefinition(targetType, propertyChangingField);

            return new EventInvokerMethod
                       {
                           MethodReference = InjectInterceptedMethod(targetType, methodDefinition).GetGeneric(),
                           IsBefore = interceptorFinder.IsBefore,
                       };
        }
        return new EventInvokerMethod
                   {
                       MethodReference = InjectMethod(targetType, eventInvokerNameResolver.EventInvokerNames.First(), propertyChangingField).GetGeneric(),
                       IsBefore= false,
                   };
    }

    MethodDefinition GetMethodDefinition(TypeDefinition targetType, FieldReference propertyChangingField)
    {
        var eventInvokerName = "Inner" + eventInvokerNameResolver.EventInvokerNames.First();
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
        var method = new MethodDefinition(eventInvokerName, GetMethodAttributes(targetType), typeSystem.Void);
        method.Parameters.Add(new ParameterDefinition("propertyName", ParameterAttributes.None, typeSystem.String));

        var handlerVariable = new VariableDefinition(msCoreReferenceFinder.PropChangingHandlerReference);
        method.Body.Variables.Add(handlerVariable);
        var boolVariable = new VariableDefinition(typeSystem.Boolean);
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
        instructions.Add(Instruction.Create(OpCodes.Newobj, msCoreReferenceFinder.ComponentModelPropertyChangingEventConstructorReference));
        instructions.Add(Instruction.Create(OpCodes.Callvirt, msCoreReferenceFinder.ComponentModelPropertyChangingEventHandlerInvokeReference));


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
        var findPropertyChangingField = targetType.Fields.FirstOrDefault(x => NotifyInterfaceFinder.IsPropertyChangingEventHandler(x.FieldType));
        if (findPropertyChangingField == null)
        {
            return null;
        }
        return findPropertyChangingField.GetGeneric();
    }

    MethodDefinition InjectInterceptedMethod(TypeDefinition targetType, MethodDefinition innerOnPropertyChanging)
    {
        delegateHolderInjector.Execute(targetType, innerOnPropertyChanging);
        var method = new MethodDefinition(eventInvokerNameResolver.EventInvokerNames.First(), GetMethodAttributes(targetType), typeSystem.Void);

        var propertyName = new ParameterDefinition("propertyName", ParameterAttributes.None, typeSystem.String);
        method.Parameters.Add(propertyName);
        if (interceptorFinder.IsBefore)
        {
            var before = new ParameterDefinition("before", ParameterAttributes.None, typeSystem.Object);
            method.Parameters.Add(before);
        }

        var action = new VariableDefinition("firePropertyChanging", msCoreReferenceFinder.ActionTypeReference);
        method.Body.Variables.Add(action);

        var variableDefinition = new VariableDefinition("delegateHolder", delegateHolderInjector.TypeDefinition);
        method.Body.Variables.Add(variableDefinition);


        var instructions = method.Body.Instructions;

        var last = Instruction.Create(OpCodes.Ret);
        instructions.Add(Instruction.Create(OpCodes.Newobj, delegateHolderInjector.ConstructorDefinition));
        instructions.Add(Instruction.Create(OpCodes.Stloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        instructions.Add(Instruction.Create(OpCodes.Stfld, delegateHolderInjector.PropertyName));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Stfld, delegateHolderInjector.Target));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldftn, delegateHolderInjector.MethodDefinition));
        instructions.Add(Instruction.Create(OpCodes.Newobj, msCoreReferenceFinder.ActionConstructorReference));
        instructions.Add(Instruction.Create(OpCodes.Stloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldfld, delegateHolderInjector.PropertyName));
        if (interceptorFinder.IsBefore)
        {
            instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            instructions.Add(Instruction.Create(OpCodes.Call, interceptorFinder.InterceptMethod));
        }
        else
        {
            instructions.Add(Instruction.Create(OpCodes.Call, interceptorFinder.InterceptMethod));
        }

        instructions.Add(last);
        method.Body.InitLocals = true;

        targetType.Methods.Add(method);
        return method;
    }
}