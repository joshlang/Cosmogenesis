using Cosmogenesis.Generator.Models.Attributes;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Models;
class ClassModel
{
    public INamedTypeSymbol ClassSymbol = default!;
    public List<DbAttributeModel> DbAttributes = new();
    public DocTypeAttributeModel? DocTypeAttribute;
    public MutableAttributeModel? MutableAttribute;
    public PartitionAttributeModel? PartitionAttribute;
    public PartitionDefinitionAttributeModel? PartitionDefinitionAttribute;
    public TransientAttributeModel? TransientAttribute;
    public List<PropertyModel> Properties = new();
    public List<MethodModel> Methods = new();
    public bool IsDbDoc;
}
