using HelpdeskAPI.Data;
using HelpdeskAPI.DTOs;
using HelpdeskAPI.Models;
using HelpdeskAPI.Services;
using Microsoft.AspNetCore.Authorization;
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

    // POST /api/auth/seed-agents
    // Temporary setup endpoint for demo: creates/updates agents with BCrypt passwords.
    // Remove or protect this endpoint before production.
    [HttpPost("seed-agents")]
    [AllowAnonymous]
    public async Task<IActionResult> SeedAgents([FromBody] List<SeedAgentRequest> agents)
    {
        if (agents == null || agents.Count == 0)
            return BadRequest(new { message = "La liste des agents est vide." });

        foreach (var req in agents)
        {
            var existing = await _db.Agents
                .FirstOrDefaultAsync(a => a.Login == req.Login);

            if (existing == null)
            {
                var agent = new HelpdeskAPI.Models.Agent
                {
                    Nom = req.Nom,
                    Prenom = req.Prenom,
                    Email = req.Email,
                    Login = req.Login,
                    Mdp = BCrypt.Net.BCrypt.HashPassword(req.Mdp),
                    Ref_Equ = req.Ref_Equ,
                    Id_Rol = req.Id_Rol
                };

                _db.Agents.Add(agent);
            }
            else
            {
                existing.Nom = req.Nom;
                existing.Prenom = req.Prenom;
                existing.Email = req.Email;
                existing.Mdp = BCrypt.Net.BCrypt.HashPassword(req.Mdp);
                existing.Ref_Equ = req.Ref_Equ;
                existing.Id_Rol = req.Id_Rol;
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Agents créés/mis à jour avec succès." });
    }
}

public record SeedAgentRequest(
    string Nom,
    string Prenom,
    string Email,
    string Login,
    string Mdp,
    int Ref_Equ,
    int Id_Rol
);
