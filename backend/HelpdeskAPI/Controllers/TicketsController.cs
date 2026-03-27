using System.Security.Claims;
using HelpdeskAPI.Data;
using HelpdeskAPI.DTOs;
using HelpdeskAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskAPI.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TicketService _ticketService;

    public TicketsController(AppDbContext db, TicketService ticketService)
    {
        _db = db;
        _ticketService = ticketService;
    }

    // GET /api/tickets  — Client sees own tickets; Agent sees tickets assigned to them
    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);
        var role   = User.FindFirst(ClaimTypes.Role)!.Value;

        IQueryable<Models.Ticket> query = _db.Tickets
            .Include(t => t.Statut)
            .Include(t => t.Priorite)
            .Include(t => t.Categorie).ThenInclude(c => c!.Equipe)
            .Include(t => t.Client)
            .Include(t => t.Affectations).ThenInclude(af => af.Agent);

        if (role == "Client")
            query = query.Where(t => t.Id_Cli == userId);
        else
            query = query.Where(t => t.Affectations.Any(af => af.Id_Agt == userId));

        var tickets = await query.OrderByDescending(t => t.Datecre).ToListAsync();

        return Ok(tickets.Select(t => MapToDto(t)));
    }

    // GET /api/tickets/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicket(int id)
    {
        var ticket = await _db.Tickets
            .Include(t => t.Statut)
            .Include(t => t.Priorite)
            .Include(t => t.Categorie).ThenInclude(c => c!.Equipe)
            .Include(t => t.Client)
            .Include(t => t.Affectations).ThenInclude(af => af.Agent)
            .FirstOrDefaultAsync(t => t.Num_Tic == id);

        if (ticket == null) return NotFound();

        // Security: client can only see own tickets
        var userId = int.Parse(User.FindFirst("userId")!.Value);
        var role   = User.FindFirst(ClaimTypes.Role)!.Value;
        if (role == "Client" && ticket.Id_Cli != userId) return Forbid();

        return Ok(MapToDto(ticket));
    }

    // POST /api/tickets  — Client only
    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest req)
    {
        var clientId = int.Parse(User.FindFirst("userId")!.Value);

        var ticket = await _ticketService.CreateAndAssignTicketAsync(
            clientId, req.Titre, req.Descr, req.Id_Pri, req.Cod_Cat);

        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Num_Tic },
            new { ticket.Num_Tic });
    }

    // PUT /api/tickets/{id}/status  — Agent only
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Agent,Superviseur")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTicketStatusRequest req)
    {
        var agentId = int.Parse(User.FindFirst("userId")!.Value);
        var ok = await _ticketService.ChangeStatutAsync(id, req.Id_Sta_Apres, agentId);
        return ok ? NoContent() : NotFound();
    }

    // GET /api/tickets/{id}/history
    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetHistory(int id)
    {
        var history = await _db.Changements
            .Include(ch => ch.StatutAvant)
            .Include(ch => ch.StatutApres)
            .Include(ch => ch.Agent)
            .Where(ch => ch.Num_Tic == id)
            .OrderBy(ch => ch.Date_Change)
            .ToListAsync();

        return Ok(history.Select(h => new ChangerDto(
            h.Id_Cha,
            h.StatutAvant?.Libelle,
            h.StatutApres!.Libelle,
            h.Date_Change,
            h.Agent != null ? $"{h.Agent.Prenom} {h.Agent.Nom}" : "Système"
        )));
    }

    private static TicketDto MapToDto(Models.Ticket t)
    {
        var agentAssigne = t.Affectations.FirstOrDefault()?.Agent;
        return new TicketDto(
            t.Num_Tic,
            t.Titre,
            t.Descr,
            t.Datecre,
            t.Statut?.Libelle ?? "",
            t.Priorite?.Libelle ?? "",
            t.Categorie?.Libelle ?? "",
            t.Categorie?.Equipe?.Libelle ?? "",
            t.Client != null ? $"{t.Client.Prenom} {t.Client.Nom}" : "",
            agentAssigne != null
                ? new DTOs.Agent(agentAssigne.Id_Agt, $"{agentAssigne.Prenom} {agentAssigne.Nom}")
                : null
        );
    }
}
