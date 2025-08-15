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
        public IActionResult GetAllClients(
            string? clientLogin = null
        )
        {
            var query = _context.Clients.AsQueryable();
            var isClientLogin = string.IsNullOrEmpty(clientLogin);

            if (!isClientLogin)
                query = query.Where(c => c.Login == clientLogin);

            var clients = query.Select(
                c => new ClientDto
                {
                    Id = c.Id,
                    Login = c.Login,
                    FullName = c.FullName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber
                }
            );
            if (!isClientLogin)
                return Ok(clients.FirstOrDefault());
            else
                return Ok(clients.ToList());
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
            bool loginExists = _context.Clients.Any(c => c.Login == dto.Login);
            if (loginExists) return BadRequest("Клиент с таким логином уже существует!");

            bool emailExists = _context.Clients.Any(c => c.Email == dto.Email);
            if (emailExists) return BadRequest("Клиент с таким email уже существует!");

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
            bool loginExists = _context.Clients.Any(c => c.Login == dto.Login && c.Id != id);
            if (loginExists) return BadRequest("Клиент с таким логином уже существует!");

            bool emailExists = _context.Clients.Any(c => c.Email == dto.Email && c.Id != id);
            if (emailExists) return BadRequest("Клиент с таким email уже существует!");

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