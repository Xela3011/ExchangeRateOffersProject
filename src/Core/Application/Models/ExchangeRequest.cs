namespace Core.Application.Models
{
    public sealed record ExchangeRequest(string SourceCurrency, string TargetCurrency, decimal Amount);
}
