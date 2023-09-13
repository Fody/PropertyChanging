using System;
using System.Collections.Generic;
using System.Linq;

public partial class ModuleWeaver
{
    public List<string> EventInvokerNames = new()
    {
        "OnPropertyChanging",
        "raisePropertyChanging"
    };

    public void ResolveEventInvokerName()
    {
        var eventInvokerAttribute = Config?.Attributes("EventInvokerNames").FirstOrDefault();
        if (eventInvokerAttribute != null)
        {
            EventInvokerNames.InsertRange(0, eventInvokerAttribute.Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => _.Length > 0).ToList());
        }
    }
}