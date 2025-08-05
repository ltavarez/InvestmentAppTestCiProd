namespace InvestmentApp.Core.Application.ViewModels
{
    public class BasicViewModel<TKey>
    {
        public required TKey Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}

