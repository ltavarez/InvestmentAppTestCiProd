namespace InvestmentApp.Core.Application.Dtos
{
    public class BasicDto<TKey>
    {
        public required TKey Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
