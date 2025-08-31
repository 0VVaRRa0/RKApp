using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using ServerAPI.Dtos;
using ServerAPI.Entities;
using ServerAPI.SignalR;

namespace ServerAPI.Controllers;

[ApiController]
[Route("/api/services/")]
public class ServiceController : ControllerBase
{
    private readonly RkdbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly IHubContext<ServerHub> _hub;
    public ServiceController(RkdbContext context, IMapper mapper, IMemoryCache cache, IHubContext<ServerHub> hub)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
        _hub = hub;
    }

    [HttpGet]
    public IActionResult GetServices(string? serviceName)
    {
        if (!string.IsNullOrEmpty(serviceName))
        {
            var filtered = _context.Services
            .AsEnumerable()
            .Where(s => s.Name.Contains(serviceName, StringComparison.OrdinalIgnoreCase)).ToList()
            .Select(s => _mapper.Map<ServiceDto>(s))
            .ToList();

            return Ok(filtered);
        }

        var cacheKey = "AllServices";
        if (!_cache.TryGetValue(cacheKey, out List<ServiceDto>? allServices))
        {
            allServices = _context.Services.Select(s => _mapper.Map<ServiceDto>(s)).ToList();
            _cache.Set(cacheKey, allServices, new MemoryCacheEntryOptions
            { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
        }

        return Ok(allServices);
    }

    [HttpGet("{id}")]
    public IActionResult GetServiceById(int id)
    {
        var service = _context.Services.Find(id);
        if (service == null) return NotFound();
        var dto = _mapper.Map<ServiceDto>(service);

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateService(ServiceDto dto)
    {
        var service = _mapper.Map<Service>(dto);

        _context.Services.Add(service);
        _context.SaveChanges();
        _cache.Remove("AllServices");
        await _hub.Clients.All.SendAsync("ServicesUpdated");

        dto.Id = service.Id;
        return CreatedAtAction(nameof(GetServiceById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(int id, ServiceDto dto)
    {
        var service = _context.Services.Find(id);
        if (service == null) return NotFound();
        _mapper.Map(dto, service);

        _context.SaveChanges();
        _cache.Remove("AllServices");
        await _hub.Clients.All.SendAsync("ServicesUpdated");

        dto.Id = service.Id;
        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var service = _context.Services.Find(id);
        if (service == null) return NotFound();

        _context.Services.Remove(service);
        _context.SaveChanges();
        _cache.Remove("AllServices");
        await _hub.Clients.All.SendAsync("ServicesUpdated");

        return NoContent();
    }
}
