namespace BankingSystem.Domain.Common
{
    public  abstract class BaseEntity
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public DateTime CreatedAt { get;private set; }
        public DateTime UpdatedAt { get; private set; }
        public byte[] RowVersion { get; private set; }

        protected void UpdateTimeStamp()        
        {
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
