using Microsoft.AspNetCore.Mvc;
using ProcessControl.API.Models;

namespace ProcessControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    // Lista em memória simulada para testes rápidos de utilizador
    private static readonly List<User> Users = new()
    {
        new User { Id = 1, Name = "Johni Pedro", Email = "johni@empresa.com", Role = "Engenheiro de Processo" },
        new User { Id = 2, Name = "Operador CNC 01", Email = "operador1@empresa.com", Role = "Operador" }
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(Users);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound(new { Message = "Utilizador não encontrado." });

        return Ok(user);
    }

    [HttpPost]
    public IActionResult Create([FromBody] User newUser)
    {
        newUser.Id = Users.Count + 1;
        newUser.CreatedAt = DateTime.UtcNow;
        Users.Add(newUser);

        return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
    }
}
