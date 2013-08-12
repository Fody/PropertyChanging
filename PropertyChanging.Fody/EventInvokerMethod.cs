using Mono.Cecil;

public enum InvokerTypes
{
    String,
    Before,
    PropertyChangingArg
}

public class EventInvokerMethod 
{

    public MethodReference MethodReference;
    public InvokerTypes InvokerType;
}