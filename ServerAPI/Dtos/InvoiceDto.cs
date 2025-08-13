namespace ServerAPI.Dtos;

public class InvoiceDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int ClientId { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? ReceiptNumber { get; set; }
    public string Status { get; set; } = null!;
}

