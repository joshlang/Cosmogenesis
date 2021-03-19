namespace Cosmogenesis.Core.Tests
{
    class TestDoc : DbDoc
    {
        public static readonly TestDoc Instance = new TestDoc();

        protected override bool ValidateState() => true;
    }
}
