using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator
{
    static class AccessibilityExtensions
    {
        public static bool IsAccessible(this Accessibility a) => 
            a == Accessibility.Public || 
            a == Accessibility.Internal ||
            a == Accessibility.ProtectedAndInternal ||
            a == Accessibility.ProtectedOrInternal;
    }
}
