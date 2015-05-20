using System;
using Microsoft.CodeAnalysis;

namespace VSDiagnostics.Utilities
{
    public static class Extensions
    {
        public static bool InheritsFrom(this ISymbol typeSymbol, Type type)
        {
            if (typeSymbol == null || type == null)
            {
                return false;
            }

            var baseType = typeSymbol;
            while (baseType != null)
            {
                if (baseType.MetadataName == type.Name)
                {
                    return true;
                }
                baseType = ((ITypeSymbol) typeSymbol).BaseType;
            }

            return false;
        }
    }
}