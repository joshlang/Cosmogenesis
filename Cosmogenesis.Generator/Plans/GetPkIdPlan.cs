namespace Cosmogenesis.Generator.Plans;
class GetPkIdPlan
{
    public class Argument
    {
        public string FullTypeName = default!;
        public string ArgumentName = default!;
        public string PropertyName = default!;
    }
    public string FullMethodName = default!;
    public List<Argument> Arguments = default!;
}
