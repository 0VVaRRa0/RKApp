using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServerAPI.Dtos;
using ServerAPI.Entities;
using ServerAPI.SignalR;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoiceController : ControllerBase
{
    private readonly RkdbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly IHubContext<ServerHub> _hub;
    public InvoiceController(RkdbContext context, IMapper mapper, IMemoryCache cache, IHubContext<ServerHub> hub)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
        _hub = hub;
    }

    [HttpGet]
    public IActionResult GetInvoices(
        DateTime? issueDate = null,
        DateTime? paymentDate = null,
        int? serviceId = null,
        int? clientId = null,
        string? serviceName = null,
        string? clientLogin = null,
        bool? status = null
    )
    {
        if (issueDate.HasValue
            || paymentDate.HasValue
            || clientId.HasValue
            || serviceId.HasValue
            || !string.IsNullOrEmpty(serviceName)
            || !string.IsNullOrEmpty(clientLogin)
            || status.HasValue)
        {
            var filtered = _context.Invoices
            .Include(i => i.Client)
            .Include(i => i.Service)
            .AsEnumerable()
            .Where(inv =>
                (!issueDate.HasValue || inv.IssueDate.Date == issueDate.Value.Date) &&
                (!paymentDate.HasValue || inv.PaymentDate == paymentDate.Value.Date) &&
                (!clientId.HasValue || inv.ClientId == clientId.Value) &&
                (!serviceId.HasValue || inv.ServiceId == serviceId.Value) &&
                (string.IsNullOrEmpty(serviceName) || inv.Service.Name.Contains(serviceName, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(clientLogin) || inv.Client.Login.Contains(clientLogin, StringComparison.OrdinalIgnoreCase)) &&
                (!status.HasValue || inv.Status == status.Value)
            )
            .Select(inv => _mapper.Map<InvoiceDto>(inv))
            .ToList();

            return Ok(filtered);
        }

        var cacheKey = "AllInvoices";
        if (!_cache.TryGetValue(cacheKey, out List<InvoiceDto>? allInvoices))
        {
            allInvoices = _context.Invoices
                    .Include(inv => inv.Service)
                    .Include(inv => inv.Client)
                    .Select(inv => _mapper.Map<InvoiceDto>(inv))
                    .ToList();
            _cache.Set(cacheKey, allInvoices, new MemoryCacheEntryOptions
            { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
        }

        return Ok(allInvoices);
    }

    [HttpGet("{id}")]
    public IActionResult GetInvoiceById(int id)
    {
        var invoice = _context.Invoices
        .Include(inv => inv.Service)
        .Include(inv => inv.Client)
        .FirstOrDefault(inv => inv.Id == id);
        var dto = _mapper.Map<InvoiceDto>(invoice);

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateInvoice(InvoiceDto dto)
    {
        var invoice = _mapper.Map<Invoice>(dto);
        _context.Invoices.Add(invoice);
        _context.SaveChanges();
        _cache.Remove("AllInvoices");
        await _hub.Clients.All.SendAsync("InvoicesUpdated");

        dto.Id = invoice.Id;
        return CreatedAtAction(nameof(GetInvoiceById), new { Id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInvoice(int id, InvoiceDto dto)
    {
        var invoice = _context.Invoices.Find(id);
        if (invoice == null) return NotFound();
        _mapper.Map(dto, invoice);
        _context.SaveChanges();
        _cache.Remove("AllInvoices");
        await _hub.Clients.All.SendAsync("InvoicesUpdated");

        dto.Id = invoice.Id;
        return Ok(dto);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
        var invoice = _context.Invoices.Find(id);
        if (invoice == null) return NotFound();
        _context.Invoices.Remove(invoice);
        _context.SaveChanges();
        _cache.Remove("AllInvoices");
        await _hub.Clients.All.SendAsync("InvoicesUpdated");

        return NoContent();
    }
}
