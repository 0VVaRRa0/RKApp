namespace Accounting.Dtos;

public class InvoiceDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int ClientId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? PaymentDate { get; set; }
    public string ReceiptNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
}

