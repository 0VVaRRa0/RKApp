namespace ServerAPI.Entities;

public partial class Invoice
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int ServiceId { get; set; }

    public decimal Amount { get; set; }

    public bool Status { get; set; }

    public string? ReceiptNumber { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
