namespace OrderEventProcessor.Models;

public class PaymentEvent
{
    public string OrderId { get; set; } // Order identification, e.g. O-123
    public decimal Amount { get; set; } // Amount paid, e.g. 11.00
}
