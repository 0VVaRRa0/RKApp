using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Dtos;
using ServerAPI.Entities;

namespace ServerAPI.Controllers
{
    [ApiController]
    [Route("api/services")]
    public class ServiceController : ControllerBase
    {
        private readonly RkappDbContext _context;
        public ServiceController(RkappDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAllServices()
        {
            var services = _context.Services
            .Select(s => new ServiceDto { Id = s.Id, Name = s.Name })
            .ToList();
            return Ok(services);
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
            var service = new Service
            {
                Name = dto.Name
            };
            _context.Services.Add(service);
            _context.SaveChanges();
            dto.Id = service.Id;
            return CreatedAtAction(nameof(GetServiceById), new { id = dto.Id }, dto);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateService(int id, ServiceDto dto)
        {
            var service = _context.Services.Find(id);
            if (service == null) return NotFound();
            service.Name = dto.Name;
            dto.Id = service.Id;
            _context.SaveChanges();
            return Ok(dto);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteService(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null) return NotFound();
            _context.Services.Remove(service);
            _context.SaveChanges();
            return NoContent();
        }
    }
}