using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ServerAPI.Dtos;
using ServerAPI.Entities;

namespace ServerAPI.Controllers;

[ApiController]
[Route("/api/clients")]
public class ClientController : Controller
{
    private readonly RkdbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    public ClientController(RkdbContext context, IMapper mapper, IMemoryCache cache)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult GetClients(
        string? login,
        string? fullName,
        string? phone,
        string? email
    )
    {
        if (!string.IsNullOrEmpty(login)
            || !string.IsNullOrEmpty(fullName)
            || !string.IsNullOrEmpty(phone)
            || !string.IsNullOrEmpty(email))
        {
            var filtered = _context.Clients
            .Where(c =>
                (string.IsNullOrEmpty(login) || c.Login.Contains(login, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(fullName) || c.FullName.Contains(fullName, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(phone) || c.Phone.Contains(phone, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(email) || c.Email.Contains(email, StringComparison.OrdinalIgnoreCase))
            )
            .Select(c => _mapper.Map<ClientDto>(c))
            .ToList();

            return Ok(filtered);
        }

        var cacheKey = "AllClients";
        if (!_cache.TryGetValue(cacheKey, out List<ClientDto>? allClients))
        {
            allClients = _context.Clients.Select(c => _mapper.Map<ClientDto>(c)).ToList();
            _cache.Set(cacheKey, allClients, new MemoryCacheEntryOptions
            { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
        }

            return Ok(allClients);
    }

    [HttpGet("{id}")]
    public IActionResult GetClientById(int id)
    {
        var client = _context.Clients.Find(id);
        if (client == null) return NotFound();
        var dto = _mapper.Map<ClientDto>(client);

        return Ok(dto);
    }

    [HttpPost]
    public IActionResult CreateClient(ClientDto dto)
    {
        var client = _mapper.Map<Client>(dto);
        _context.Clients.Add(client);
        _context.SaveChanges();
        _cache.Remove("AllClients");

        dto.Id = client.Id;
        return CreatedAtAction(nameof(GetClientById), new {id = client.Id}, dto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateClient(int id, ClientDto dto)
    {
        var client = _context.Clients.Find(id);
        if (client == null) return NotFound();
        _mapper.Map(dto, client);

        _context.SaveChanges();
        _cache.Remove("AllClients");

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteClient(int id)
    {
        var client = _context.Clients.Find(id);
        if (client == null) return NotFound();

        _context.Clients.Remove(client);
        _context.SaveChanges();
        _cache.Remove("AllClients");

        return NoContent();
    }
}
