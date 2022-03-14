using Cosmogenesis.Generator.Models;

namespace Cosmogenesis.Generator.Plans;
class PropertyPlan
{
    public PropertyModel PropertyModel = default!;
    public string PropertyName = default!;
    public string ArgumentName = default!;
    public string FullTypeName = default!;
    public bool UseDefault;
    public bool IsInitOnly;
}
