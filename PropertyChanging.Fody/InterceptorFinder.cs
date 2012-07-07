using System.Linq;
using Mono.Cecil;


public class InterceptorFinder
{
    ModuleWeaver moduleWeaver;
    public bool Found;
    public MethodDefinition InterceptMethod;
    public bool IsBefore;

    public InterceptorFinder(ModuleWeaver moduleWeaver)
    {
        this.moduleWeaver = moduleWeaver;
    }


    void SearchForMethod(TypeDefinition typeDefinition)
    {
        var methodDefinition = typeDefinition.Methods.FirstOrDefault(x => x.Name == "Intercept");
        if (methodDefinition == null)
        {
            throw new WeavingException(string.Format("Found Type '{0}' but could not find a method named 'Intercept'.", typeDefinition.FullName));
        }
        if (!methodDefinition.IsStatic)
        {
            throw new WeavingException(string.Format("Found Type '{0}.Intercept' but it is not static.", typeDefinition.FullName));
        }
        if (!methodDefinition.IsPublic)
        {
            throw new WeavingException(string.Format("Found Type '{0}.Intercept' but it is not public.", typeDefinition.FullName));
        }

        if (IsSingleStringMethod(methodDefinition))
        {
            Found = true;
            InterceptMethod = methodDefinition;
            return;
        }
        if (IsBeforeMethod(methodDefinition))
        {
            Found = true;
            InterceptMethod = methodDefinition;
            IsBefore = true;
            return;
        }
        var message = string.Format(
            @"Found '{0}.Intercept' But the signature is not correct. It needs to be either.
Intercept(object target, Action firePropertyChanging, string propertyName)
or
Intercept(object target, Action firePropertyChanging, string propertyName, object before)", typeDefinition.FullName);
        throw new WeavingException(message);
    }


    public bool IsSingleStringMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 3
               && parameters[0].ParameterType.FullName == "System.Object"
               && parameters[1].ParameterType.FullName == "System.Action"
               && parameters[2].ParameterType.FullName == "System.String";
    }

    public bool IsBeforeMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 4
               && parameters[0].ParameterType.FullName == "System.Object"
               && parameters[1].ParameterType.FullName == "System.Action"
               && parameters[2].ParameterType.FullName == "System.String"
               && parameters[3].ParameterType.FullName == "System.Object";
    }

    public void Execute()
    {
        var typeDefinition = moduleWeaver.ModuleDefinition.Types.FirstOrDefault(x => x.Name == "PropertyChangingNotificationInterceptor");
        if (typeDefinition == null)
        {
            Found = false;
            return;
        }
        SearchForMethod(typeDefinition);
    }
}