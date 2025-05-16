namespace OrderEventProcessor.Models;

public class OrderEvent
{
    public string Id { get; set; } // Order identification, e.g. O-123
    public string Product { get; set; } // Product identification, e.g. PR-ABC
    public decimal Total { get; set; } // Total order price, e.g. 12.34
    public string Currency { get; set; } // Currency code, e.g. USD
}