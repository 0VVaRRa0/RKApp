using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServerAPI.Dtos;
using ServerAPI.Entities;
using ServerAPI.Hubs;

namespace ServerAPI.Controllers
{
    [ApiController]
    [Route("api/services")]
    public class ServiceController : ControllerBase
    {
        private readonly RkappDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IHubContext<NotificationsHub> _hub;
        public ServiceController(RkappDbContext context, IMemoryCache cache, IHubContext<NotificationsHub> hub)
        {
            _context = context;
            _cache = cache;
            _hub = hub;
        }
        [HttpGet]
        public IActionResult GetAllServices()
        {
            if (!_cache.TryGetValue("AllServices", out List<ServiceDto>? cachedServices))
            {
                var services = _context.Services
                .Select(s => new ServiceDto { Id = s.Id, Name = s.Name })
                .ToList();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                };
                _cache.Set("AllServices", services, cacheOptions);
                cachedServices = services;
            }
            return Ok(cachedServices);
        }
        [HttpGet("{id}")]
        public IActionResult GetServiceById(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null) return NotFound();
            var dto = new ServiceDto
            {
                Id = service.Id,
                Name = service.Name
            };
            return Ok(dto);
        }
        [HttpPost]
        public IActionResult CreateService(ServiceDto dto)
        {
            bool nameExists = _context.Services.Any(s => s.Name == dto.Name);
            if (nameExists) return BadRequest("Услуга с таким названием уже существует!");

            var service = new Service
            {
                Name = dto.Name
            };
            _context.Services.Add(service);
            _context.SaveChanges();
            dto.Id = service.Id;

            _cache.Remove("AllServices");

            _hub.Clients.All.SendAsync("RefreshServices");

            return CreatedAtAction(nameof(GetServiceById), new { id = dto.Id }, dto);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateService(int id, ServiceDto dto)
        {
            bool nameExists = _context.Services.Any(s => s.Name == dto.Name && s.Id != id);
            if (nameExists) return BadRequest("Услуга с таким названием уже существует!");

            var service = _context.Services.Find(id);
            if (service == null) return NotFound();
            service.Name = dto.Name;
            dto.Id = service.Id;
            _context.SaveChanges();

            _cache.Remove("AllServices");

            _hub.Clients.All.SendAsync("RefreshServices");

            return Ok(dto);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteService(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null) return NotFound();
            _context.Services.Remove(service);
            _context.SaveChanges();

            _cache.Remove("AllServices");

            _hub.Clients.All.SendAsync("RefreshServices");
            
            return NoContent();
        }
    }
}