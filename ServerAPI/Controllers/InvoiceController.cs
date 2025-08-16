using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Entities;
using ServerAPI.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.SignalR;
using ServerAPI.Hubs;
using Microsoft.EntityFrameworkCore;

namespace ServerAPI.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly RkappDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IHubContext<NotificationsHub> _hub;
        public InvoiceController(RkappDbContext context, IMemoryCache cache, IHubContext<NotificationsHub> hub)
        {
            _context = context;
            _cache = cache;
            _hub = hub;
        }
        [HttpGet]
        public IActionResult GetAllInvoices(
            DateTime? issueDate = null,
            DateTime? paymentDate = null,
            int? serviceId = null,
            string? ServiceName = null,
            int? clientId = null,
            string? clientLogin = null,
            string? status = null)
        {
            if (!_cache.TryGetValue("AllInvoices", out List<InvoiceDto>? cachedInvoices))
            {
                var invoices = _context.Invoices.Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    ServiceId = i.ServiceId,
                    ServiceName = i.Service.Name,
                    ClientId = i.ClientId,
                    ClientLogin = i.Client.Login,
                    Amount = i.Amount,
                    IssueDate = i.IssueDate,
                    DueDate = i.DueDate,
                    PaymentDate = i.PaymentDate,
                    ReceiptNumber = i.ReceiptNumber,
                    Status = i.Status
                }).ToList();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                };
                _cache.Set("AllInvoices", invoices, cacheOptions);
                cachedInvoices = invoices;
            }

            var query = cachedInvoices!.AsQueryable();

            if (issueDate.HasValue)
                query = query.Where(i => i.IssueDate == issueDate.Value);

            if (paymentDate.HasValue)
                query = query.Where(i => i.PaymentDate == paymentDate.Value);

            if (serviceId.HasValue)
                query = query.Where(i => i.ServiceId == serviceId);

            if (!string.IsNullOrEmpty(ServiceName))
                query = query.Where(i => i.ServiceName!.Contains(ServiceName));

            if (!string.IsNullOrEmpty(clientLogin))
                query = query.Where(i => i.ClientLogin!.Contains(clientLogin));

            if (clientId.HasValue)
                query = query.Where(i => i.ClientId == clientId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(i => i.Status == status);

            return Ok(query.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetInvoiceById(int id)
        {
            var invoice = _context.Invoices
            .Include(i => i.Client)
            .Include(i => i.Service)
            .FirstOrDefault(i => i.Id == id);
            if (invoice == null) return NotFound();
            var dto = new InvoiceDto
            {
                Id = invoice.Id,
                ServiceId = invoice.ServiceId,
                ServiceName = invoice.Service.Name,
                ClientId = invoice.ClientId,
                ClientLogin = invoice.Client.Login,
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
            bool clientExists = _context.Clients.Any(c => c.Id == dto.ClientId);
            if (!clientExists) return BadRequest($"Клиент с ID={dto.ClientId} не существует!");

            bool serviceExists = _context.Services.Any(s => s.Id == dto.ServiceId);
            if (!serviceExists) return BadRequest($"Услуга с ID={dto.ServiceId} не существует!");

            bool receiptNumberExists = _context.Invoices.Any(inv => inv.ReceiptNumber == dto.ReceiptNumber && dto.ReceiptNumber == "");
            if (receiptNumberExists) return BadRequest($"Счёт с номером квитанции: {dto.ReceiptNumber} уже существует!");

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
            _cache.Remove("AllInvoices");
            _hub.Clients.All.SendAsync("RefreshInvoices");
            return CreatedAtAction(nameof(GetInvoiceById), new { Id = invoice.Id }, dto);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateInvoice(int id, InvoiceDto dto)
        {
            bool clientExists = _context.Clients.Any(c => c.Id == dto.ClientId);
            if (!clientExists) return BadRequest($"Клиент с ID={dto.ClientId} не существует!");

            bool serviceExists = _context.Services.Any(s => s.Id == dto.ServiceId);
            if (!serviceExists) return BadRequest($"Услуга с ID={dto.ServiceId} не существует!");

            bool receiptNumberExists = _context.Invoices.Any(inv => inv.ReceiptNumber == dto.ReceiptNumber && inv.Id != id && dto.ReceiptNumber == "");
            if (receiptNumberExists) return BadRequest($"Счёт с номером квитанции: {dto.ReceiptNumber} уже существует!");

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
            _cache.Remove("AllInvoices");
            _hub.Clients.All.SendAsync("RefreshInvoices");
            return Ok(dto);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteInvoice(int id)
        {
            var invoice = _context.Invoices.Find(id);
            if (invoice == null) return NotFound();
            _context.Invoices.Remove(invoice);
            _context.SaveChanges();
            _cache.Remove("AllInvoices");
            _hub.Clients.All.SendAsync("RefreshInvoices");
            return NoContent();
        }
    }
}