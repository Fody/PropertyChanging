using System.Collections.Generic;
using Mono.Cecil;

public class NotifyPropertyData
{
    public NotifyPropertyData()
    {
        AlsoNotifyFor = new();
    }

    public List<PropertyDefinition> AlsoNotifyFor;
}