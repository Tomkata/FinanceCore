public record SearchTransactionsQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public Guid? AccountId { get; set; }
    public string? TransactionType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? SortOrder { get; set; }
}