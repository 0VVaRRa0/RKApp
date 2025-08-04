using System;
using System.Collections.Generic;

namespace ServerAPI.Entities;

public partial class Invoice
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

    public virtual Service Service { get; set; } = null!;
}
