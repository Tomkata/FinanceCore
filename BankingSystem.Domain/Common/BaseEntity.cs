namespace BankingSystem.Domain.Common
{
    public  abstract class BaseEntity
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            // Ensure RowVersion is non-null so SQLite (which doesn't auto-generate SQL Server rowversion) won't fail NOT NULL inserts in tests
            RowVersion = new byte[8];   
        }
        public Guid Id { get; set; }            
        public DateTime CreatedAt { get;private set; }
        public DateTime UpdatedAt { get; private set; }
       public byte[]? RowVersion { get; private set; }  
        protected void UpdateTimeStamp()        
        {
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
