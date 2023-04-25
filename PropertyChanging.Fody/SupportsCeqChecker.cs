using System;
using System.Collections.Generic;
using Mono.Cecil;

public static class SupportsCeqChecker
{

    static List<string> ceqStructNames;

    static SupportsCeqChecker()
    {
        ceqStructNames = new()
        {
            nameof(Int32),
            nameof(UInt32),
            nameof(Int64),
            nameof(UInt64),
            nameof(Single),
            nameof(Double),
            nameof(Boolean),
            nameof(Int16),
            nameof(UInt16),
            nameof(Byte),
            nameof(SByte),
            nameof(Char),
        };
    }

    public static bool SupportsCeq(this TypeReference typeReference)
    {
        if (ceqStructNames.Contains(typeReference.Name))
        {
            return true;
        }

        var typeDefinition = typeReference.Resolve();
        if (typeDefinition.IsEnum)
        {
            return true;
        }

        return !typeDefinition.IsValueType;
    }
}