namespace AiAssistantApi.Models;

public class StockStats
{
    public string Symbol { get; set; }= string.Empty;
    public decimal Highest { get; set; } = 0m;
    public decimal Lowest { get; set; }
    public string TimeRange { get; set; }= string.Empty;
}
