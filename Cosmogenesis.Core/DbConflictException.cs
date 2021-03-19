namespace Cosmogenesis.Core
{
    public sealed class DbConflictException : DbException
    {
        public readonly DbConflictType DbConflictType;

        internal DbConflictException(DbConflictType dbConflictType) : base(message: $"Db conflict occurred: {dbConflictType}")
        {
            DbConflictType = dbConflictType;
        }
    }
}
