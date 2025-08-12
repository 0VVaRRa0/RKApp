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
    [Route("api/clients")]
    public class ClientController : ControllerBase
    {
        private readonly RkappDbContext _context;
        public ClientController(RkappDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAllClients()
        {
            var clients = _context.Clients
            .Select(
                c => new ClientDto
                {
                    Id = c.Id,
                    Login = c.Login,
                    FullName = c.FullName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber
                }
            )
            .ToList();
            return Ok(clients);
        }
        [HttpGet("{id}")]
        public IActionResult GetClientById(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null) return NotFound();
            var dto = new ClientDto
            {
                Id = client.Id,
                Login = client.Login,
                FullName = client.FullName,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber
            };
            return Ok(dto);
        }
        [HttpGet("login/{login}")]
        public IActionResult GetClientByLogin(string login)
        {
            var client = _context.Clients.SingleOrDefault(c => c.Login == login);
            if (client == null) return NotFound();
            var dto = new ClientDto
            {
                Id = client.Id,
                Login = client.Login,
                FullName = client.FullName,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber
            };
            return Ok(dto);
        }
        [HttpPost]
        public IActionResult CreateClient(ClientDto dto)
        {
            var client = new Client
            {
                Login = dto.Login,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };
            _context.Add(client);
            _context.SaveChanges();
            dto.Id = client.Id;
            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, dto);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateClient(int id, ClientDto dto)
        {
            var client = _context.Clients.Find(id);
            if (client == null) return NotFound();
            client.Login = dto.Login;
            client.FullName = dto.FullName;
            client.Email = dto.Email;
            client.PhoneNumber = dto.PhoneNumber;
            _context.SaveChanges();
            dto.Id = client.Id;
            return Ok(dto);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteClient(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null) return NotFound();
            _context.Clients.Remove(client);
            _context.SaveChanges();
            return NoContent();
        }
    }
}