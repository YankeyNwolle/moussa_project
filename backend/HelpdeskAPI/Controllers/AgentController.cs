using System.Security.Claims;
using HelpdeskAPI.Data;
using HelpdeskAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskAPI.Controllers;

[ApiController]
[Route("api/agent")]
[Authorize(Roles = "Agent,Superviseur")]
public class AgentController : ControllerBase
{
    private readonly AppDbContext _db;

    public AgentController(AppDbContext db) => _db = db;

    // GET /api/agent/dashboard
    // Returns stats: total tickets, open, resolved, closed (for this agent's équipe)
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var agentId = int.Parse(User.FindFirst("userId")!.Value);
        var role    = User.FindFirst(ClaimTypes.Role)!.Value;
        var agent   = await _db.Agents.FindAsync(agentId);
        if (agent == null) return Unauthorized();

        IQueryable<Models.Ticket> baseQuery;

        if (role == "Superviseur")
        {
            // Le superviseur voit tous les tickets
            baseQuery = _db.Tickets.AsQueryable();
        }
        else
        {
            // Un agent normal voit uniquement les tickets qui lui sont assignés
            var teamTicketIds = await _db.Affectations
                .Where(af => af.Id_Agt == agentId)
                .Select(af => af.Num_Tic)
                .ToListAsync();

            baseQuery = _db.Tickets.Where(t => teamTicketIds.Contains(t.Num_Tic));
        }

        var statuts = await _db.Statuts.ToListAsync();

        var stats = new Dictionary<string, int>();
        foreach (var s in statuts)
        {
            stats[s.Libelle] = await baseQuery
                .CountAsync(t => t.Id_Sta == s.Id_Sta);
        }

        var totalTickets = await baseQuery.CountAsync();

        return Ok(new { totalTickets, parStatut = stats });
    }

    // GET /api/agent/tickets?statut=En+cours&priorite=3&page=1
    [HttpGet("tickets")]
    public async Task<IActionResult> GetAgentTickets(
        [FromQuery] string? statut,
        [FromQuery] int? prioriteId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var agentId = int.Parse(User.FindFirst("userId")!.Value);
        var role    = User.FindFirst(ClaimTypes.Role)!.Value;

        IQueryable<Models.Ticket> query = _db.Tickets;

        // Un agent normal ne voit que ses propres tickets, un superviseur voit tout
        if (role != "Superviseur")
        {
            query = query.Where(t => t.Affectations.Any(af => af.Id_Agt == agentId));
        }

        query = query
            .Include(t => t.Statut)
            .Include(t => t.Priorite)
            .Include(t => t.Categorie).ThenInclude(c => c!.Equipe)
            .Include(t => t.Client)
            .Include(t => t.Affectations);

        if (!string.IsNullOrWhiteSpace(statut))
            query = query.Where(t => t.Statut!.Libelle == statut);

        if (prioriteId.HasValue)
            query = query.Where(t => t.Id_Pri == prioriteId.Value);

        var total = await query.CountAsync();
        var tickets = await query
            .OrderByDescending(t => t.Datecre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            items = tickets.Select(t => new TicketDto(
                t.Num_Tic,
                t.Titre,
                t.Descr,
                t.Datecre,
                t.Statut?.Libelle ?? "",
                t.Priorite?.Libelle ?? "",
                t.Categorie?.Libelle ?? "",
                t.Categorie?.Equipe?.Libelle ?? "",
                t.Client != null ? $"{t.Client.Prenom} {t.Client.Nom}" : "",
                null
            ))
        });
    }

    // GET /api/agent/referentiels  — returns statuts, priorites, categories for dropdowns
    [HttpGet("referentiels")]
    public async Task<IActionResult> Referentiels()
    {
        return Ok(new
        {
            statuts    = await _db.Statuts.ToListAsync(),
            priorites  = await _db.Priorites.ToListAsync(),
            categories = await _db.Categories.Include(c => c.Equipe).ToListAsync()
        });
    }
}
