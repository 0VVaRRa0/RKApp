using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using ServerAPI.Dtos;
using ServerAPI.Entities;
using ServerAPI.SignalR;

namespace ServerAPI.Controllers;

[ApiController]
[Route("/api/clients")]
public class ClientController : Controller
{
    private readonly RkdbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly IHubContext<ServerHub> _hub;
    public ClientController(RkdbContext context, IMapper mapper, IMemoryCache cache, IHubContext<ServerHub> hub)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
        _hub = hub;
    }

    [HttpGet]
    public IActionResult GetClients(
        string? enteredLogin,
        string? login,
        string? fullName,
        string? phone,
        string? email
    )
    {
        if (!string.IsNullOrEmpty(login)
            || !string.IsNullOrEmpty(enteredLogin)
            || !string.IsNullOrEmpty(fullName)
            || !string.IsNullOrEmpty(phone)
            || !string.IsNullOrEmpty(email))
        {
            var filtered = _context.Clients
            .AsEnumerable()
            .Where(c =>
                (string.IsNullOrEmpty(enteredLogin) || string.Equals(c.Login, enteredLogin, StringComparison.OrdinalIgnoreCase)) &&
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
    public async Task<IActionResult> CreateClient(ClientDto dto)
    {
        var client = _mapper.Map<Client>(dto);
        _context.Clients.Add(client);
        _context.SaveChanges();
        _cache.Remove("AllClients");
        await _hub.Clients.All.SendAsync("ClientsUpdated");

        dto.Id = client.Id;
        return CreatedAtAction(nameof(GetClientById), new {id = client.Id}, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(int id, ClientDto dto)
    {
        var client = _context.Clients.Find(id);
        if (client == null) return NotFound();
        _mapper.Map(dto, client);

        _context.SaveChanges();
        _cache.Remove("AllClients");
        await _hub.Clients.All.SendAsync("ClientsUpdated");

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = _context.Clients.Find(id);
        if (client == null) return NotFound();

        _context.Clients.Remove(client);
        _context.SaveChanges();
        _cache.Remove("AllClients");
        await _hub.Clients.All.SendAsync("ClientsUpdated");

        return NoContent();
    }
}
