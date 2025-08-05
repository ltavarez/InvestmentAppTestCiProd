namespace InvestmentApp.Core.Domain.Common
{
    public class BasicEntity<TKey>
    {
        public required TKey Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
