using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    Dictionary<string, bool> typeReferencesImplementingINotify = new Dictionary<string, bool>();

    public bool HierarchyImplementsINotify(TypeReference typeReference)
    {
        var fullName = typeReference.FullName;
        if (typeReferencesImplementingINotify.TryGetValue(fullName, out var implementsINotify))
        {
            return implementsINotify;
        }

        TypeDefinition typeDefinition;
        if (typeReference.IsDefinition)
        {
            typeDefinition = (TypeDefinition) typeReference;
        }
        else
        {
            typeDefinition = Resolve(typeReference);
        }

        if (HasPropertyChangingEvent(typeDefinition))
        {
            typeReferencesImplementingINotify[fullName] = true;
            return true;
        }
        if (HasPropertyChangingField(typeDefinition))
        {
            typeReferencesImplementingINotify[fullName] = true;
            return true;
        }
        var baseType = typeDefinition.BaseType;
        if (baseType == null)
        {
            typeReferencesImplementingINotify[fullName] = false;
            return false;
        }
        var baseTypeImplementsINotify = HierarchyImplementsINotify(baseType);
        typeReferencesImplementingINotify[fullName] = baseTypeImplementsINotify;
        return baseTypeImplementsINotify;
    }


    public static bool HasPropertyChangingEvent(TypeDefinition typeDefinition)
    {
        return typeDefinition.Events.Any(IsPropertyChangingEvent);
    }

    public static bool IsPropertyChangingEvent(EventDefinition eventDefinition)
    {
        return IsNamedPropertyChanging(eventDefinition) && IsPropertyChangingEventHandler(eventDefinition.EventType);
    }

    static bool IsNamedPropertyChanging(EventDefinition eventDefinition)
    {
        return eventDefinition.Name == "PropertyChanging" ||
               eventDefinition.Name == "System.ComponentModel.INotifyPropertyChanging.PropertyChanging" ||
               eventDefinition.Name == "Windows.UI.Xaml.Data.PropertyChanging";
    }

    public static bool IsPropertyChangingEventHandler(TypeReference typeReference)
    {
        return typeReference.FullName == "System.ComponentModel.PropertyChangingEventHandler" ||
               typeReference.FullName == "Windows.UI.Xaml.Data.PropertyChangingEventHandler" ||
               typeReference.FullName == "System.Runtime.InteropServices.WindowsRuntime.EventRegistrationTokenTable`1<Windows.UI.Xaml.Data.PropertyChangingEventHandler>";
    }

    static bool HasPropertyChangingField(TypeDefinition typeDefinition)
    {
        foreach (var fieldType in typeDefinition.Fields.Select(x => x.FieldType))
        {
            if (fieldType.FullName == "Microsoft.FSharp.Control.FSharpEvent`2<System.ComponentModel.PropertyChangingEventHandler,System.ComponentModel.PropertyChangingEventArgs>")
            {
                return true;
            }
            if (fieldType.FullName == "Microsoft.FSharp.Control.FSharpEvent`2<Windows.UI.Xaml.Data.PropertyChangingEventHandler,Windows.UI.Xaml.Data.PropertyChangingEventArgs>")
            {
                return true;
            }
        }
        return false;
    }
}