using HelpdeskAPI.Data;
using HelpdeskAPI.DTOs;
using HelpdeskAPI.Models;
using HelpdeskAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthController(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    // POST /api/auth/register-client
    [HttpPost("register-client")]
    public async Task<IActionResult> RegisterClient([FromBody] RegisterClientRequest req)
    {
        if (await _db.Clients.AnyAsync(c => c.Login == req.Login))
            return Conflict(new { message = "Ce login est déjà utilisé." });

        if (await _db.Clients.AnyAsync(c => c.Email == req.Email))
            return Conflict(new { message = "Cet email est déjà utilisé." });

        var client = new Client
        {
            Nom    = req.Nom,
            Prenom = req.Prenom,
            Email  = req.Email,
            Tel    = req.Tel,
            Login  = req.Login,
            Mdp    = BCrypt.Net.BCrypt.HashPassword(req.Mdp)
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(
            client.Id_Cli, client.Login, "Client",
            $"{client.Prenom} {client.Nom}");

        return Ok(new AuthResponse(token, "Client", client.Id_Cli,
            $"{client.Prenom} {client.Nom}"));
    }

    // POST /api/auth/login-client
    [HttpPost("login-client")]
    public async Task<IActionResult> LoginClient([FromBody] LoginRequest req)
    {
        var client = await _db.Clients
            .FirstOrDefaultAsync(c => c.Login == req.Login);

        if (client == null || !BCrypt.Net.BCrypt.Verify(req.Mdp, client.Mdp))
            return Unauthorized(new { message = "Login ou mot de passe incorrect." });

        var token = _jwt.GenerateToken(
            client.Id_Cli, client.Login, "Client",
            $"{client.Prenom} {client.Nom}");

        return Ok(new AuthResponse(token, "Client", client.Id_Cli,
            $"{client.Prenom} {client.Nom}"));
    }

    // POST /api/auth/login-agent
    [HttpPost("login-agent")]
    public async Task<IActionResult> LoginAgent([FromBody] LoginRequest req)
    {
        var agent = await _db.Agents
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Login == req.Login);

        if (agent == null || !BCrypt.Net.BCrypt.Verify(req.Mdp, agent.Mdp))
            return Unauthorized(new { message = "Login ou mot de passe incorrect." });

        var roleName = agent.Role?.Libelle ?? "Agent";
        var token = _jwt.GenerateToken(
            agent.Id_Agt, agent.Login, roleName,
            $"{agent.Prenom} {agent.Nom}");

        return Ok(new AuthResponse(token, roleName, agent.Id_Agt,
            $"{agent.Prenom} {agent.Nom}"));
    }
}
