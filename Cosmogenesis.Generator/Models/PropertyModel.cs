using Cosmogenesis.Generator.Models.Attributes;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Models;
class PropertyModel
{
    public IPropertySymbol PropertySymbol = default!;
    public UseDefaultAttributeModel? UseDefaultAttribute;
    public bool IsInitOnly;
}
