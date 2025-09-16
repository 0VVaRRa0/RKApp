namespace Accounting.Dtos;

public class InvoiceDto
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int ServiceId { get; set; }

    public ClientDto? Client { get; set; }

    public ServiceDto? Service { get; set; }

    public decimal Amount { get; set; }

    public bool Status { get; set; }

    public string? ReceiptNumber { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string ClientLogin => Client?.Login ?? string.Empty;

    public string ServiceName => Service?.Name ?? string.Empty;
}
