using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Entities;
using ServerAPI.Dtos;

namespace ServerAPI.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly RkappDbContext _context;
        public InvoiceController(RkappDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAllInvoices(
            DateTime? issueDate = null,
            DateTime? paymentDate = null,
            int? serviceId = null,
            int? clientId = null,
            string? status = null)
        {
            var query = _context.Invoices.AsQueryable();
            if (issueDate.HasValue)
                query = query.Where(i => i.IssueDate == issueDate.Value);

            if (paymentDate.HasValue)
                query = query.Where(i => i.PaymentDate == paymentDate.Value);

            if (serviceId.HasValue)
                query = query.Where(i => i.ServiceId == serviceId);
                
            if (clientId.HasValue)
                query = query.Where(i => i.ClientId == clientId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(i => i.Status == status);
            var invoices = query.Select(
                i => new InvoiceDto
                {
                    Id = i.Id,
                    ServiceId = i.ServiceId,
                    ClientId = i.ClientId,
                    Amount = i.Amount,
                    IssueDate = i.IssueDate,
                    DueDate = i.DueDate,
                    PaymentDate = i.PaymentDate,
                    ReceiptNumber = i.ReceiptNumber,
                    Status = i.Status
                }
            )
            .ToList();
            return Ok(invoices);
        }
        [HttpGet("{id}")]
        public IActionResult GetInvoiceById(int id)
        {
            var invoice = _context.Invoices.Find(id);
            if (invoice == null) return NotFound();
            var dto = new InvoiceDto
            {
                Id = invoice.Id,
                ServiceId = invoice.ServiceId,
                ClientId = invoice.ClientId,
                Amount = invoice.Amount,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                PaymentDate = invoice.PaymentDate,
                ReceiptNumber = invoice.ReceiptNumber,
                Status = invoice.Status
            };
            return Ok(dto);
        }
        [HttpPost]
        public IActionResult CreateInvoice(InvoiceDto dto)
        {
            var clientExists = _context.Clients.Any(c => c.Id == dto.ClientId);
            if (!clientExists) return BadRequest($"Client with Id={dto.ClientId} does not exist");

            var serviceExists = _context.Services.Any(s => s.Id == dto.ServiceId);
            if (!serviceExists) return BadRequest($"Service with Id={dto.ServiceId} does not exist");

            var invoice = new Invoice
            {
                ServiceId = dto.ServiceId,
                ClientId = dto.ClientId,
                Amount = dto.Amount,
                IssueDate = dto.IssueDate,
                DueDate = dto.DueDate,
                ReceiptNumber = dto.ReceiptNumber,
                Status = dto.Status
            };
            _context.Invoices.Add(invoice);
            _context.SaveChanges();
            dto.Id = invoice.Id;
            dto.PaymentDate = invoice.PaymentDate;
            return CreatedAtAction(nameof(GetInvoiceById), new { Id = invoice.Id }, dto);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateInvoice(int id, InvoiceDto dto)
        {
            var clientExists = _context.Clients.Any(c => c.Id == dto.ClientId);
            if (!clientExists) return BadRequest($"Client with Id={dto.ClientId} does not exist");

            var serviceExists = _context.Services.Any(s => s.Id == dto.ServiceId);
            if (!serviceExists) return BadRequest($"Service with Id={dto.ServiceId} does not exist");

            var invoice = _context.Invoices.Find(id);
            if (invoice == null) return NotFound();
            invoice.ServiceId = dto.ServiceId;
            invoice.ClientId = dto.ClientId;
            invoice.Amount = dto.Amount;
            invoice.IssueDate = dto.IssueDate;
            invoice.DueDate = dto.DueDate;
            invoice.PaymentDate = dto.PaymentDate;
            invoice.ReceiptNumber = dto.ReceiptNumber;
            invoice.Status = dto.Status;
            _context.SaveChanges();
            dto.Id = invoice.Id;
            return Ok(dto);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteInvoice(int id)
        {
            var invoice = _context.Invoices.Find(id);
            if (invoice == null) return NotFound();
            _context.Invoices.Remove(invoice);
            _context.SaveChanges();
            return NoContent();
        }
    }
}