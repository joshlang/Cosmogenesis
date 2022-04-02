namespace Cosmogenesis.Generator.Plans;
class UnionPlan
{
    public string CommonName = default!;
    public string FullCommonTypeName = default!;
    public GetPkIdPlan GetIdPlan = default!;
    public List<DocumentPlan> Documents = default!;
}
