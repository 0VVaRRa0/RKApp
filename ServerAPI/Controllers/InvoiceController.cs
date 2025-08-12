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
        public IActionResult GetAllInvoices()
        {
            var invoices = _context.Invoices
            .Select(
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
        [HttpGet("client_{id}")]
        public IActionResult GetByClient(int id)
        {
            var invoices = _context.Invoices
            .Where(inv => inv.ClientId == id)
            .Select(inv => new InvoiceDto
            {
                Id = inv.Id,
                ServiceId = inv.ServiceId,
                ClientId = inv.ClientId,
                Amount = inv.Amount,
                IssueDate = inv.IssueDate,
                DueDate = inv.DueDate,
                PaymentDate = inv.PaymentDate,
                ReceiptNumber = inv.ReceiptNumber,
                Status = inv.Status
            })
            .ToList();
            return Ok(invoices);
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