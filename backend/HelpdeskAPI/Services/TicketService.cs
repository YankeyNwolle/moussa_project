using HelpdeskAPI.Data;
using HelpdeskAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskAPI.Services;

/// <summary>
/// Automatically assigns the least-loaded agent from the team associated
/// with the ticket's category.
/// </summary>
public class TicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Auto-assign: picks the agent in the category's équipe with the fewest
    /// open tickets (statuts 1=Initialisé, 2=En cours).
    /// Creates an AFFECTER record and a CHANGER record (initial status).
    /// </summary>
    public async Task<Ticket> CreateAndAssignTicketAsync(
        int clientId, string titre, string descr, int prioriteId, int categorieId)
    {
        // 1. Find initial statut (Initialisé = id 1 by seed order)
        var statutInit = await _db.Statuts.FirstAsync(s => s.Libelle == "Initialisé");

        // 2. Create ticket
        var ticket = new Ticket
        {
            Titre   = titre,
            Descr   = descr,
            Datecre = DateTime.UtcNow,
            Id_Cli  = clientId,
            Cod_Cat = categorieId,
            Id_Pri  = prioriteId,
            Id_Sta  = statutInit.Id_Sta
        };
        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        // 3. Record initial status in CHANGER (no avant, no agent)
        _db.Changements.Add(new Changer
        {
            Num_Tic     = ticket.Num_Tic,
            Id_Sta_Avant = null,
            Id_Sta_Apres = statutInit.Id_Sta,
            Date_Change  = DateTime.UtcNow,
            Id_Agt       = null
        });

        // 4. Find team for this category
        var categorie = await _db.Categories
            .Include(c => c.Equipe)
            .FirstAsync(c => c.Cod_Cat == categorieId);

        // 5. Pick least-busy agent in the team
        var openStatutIds = await _db.Statuts
            .Where(s => s.Libelle == "Initialisé" || s.Libelle == "En cours")
            .Select(s => s.Id_Sta)
            .ToListAsync();

        var agentAssigne = await _db.Agents
            .Where(a => a.Ref_Equ == categorie.Ref_Equ)
            .OrderBy(a => _db.Affectations
                .Count(af => af.Id_Agt == a.Id_Agt &&
                             openStatutIds.Contains(
                                 _db.Tickets.First(t => t.Num_Tic == af.Num_Tic).Id_Sta)))
            .FirstOrDefaultAsync();

        // Fallback: if no agent exists in the category's team,
        // assign the least-busy agent among ALL agents.
        if (agentAssigne == null)
        {
            agentAssigne = await _db.Agents
                .OrderBy(a => _db.Affectations
                    .Count(af => af.Id_Agt == a.Id_Agt &&
                                 openStatutIds.Contains(
                                     _db.Tickets.First(t => t.Num_Tic == af.Num_Tic).Id_Sta)))
                .FirstOrDefaultAsync();
        }

        if (agentAssigne != null)
        {
            _db.Affectations.Add(new Affecter
            {
                Num_Tic          = ticket.Num_Tic,
                Id_Agt           = agentAssigne.Id_Agt,
                Date_Affectation = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
        return ticket;
    }

    /// <summary>
    /// Change ticket status. Records history in CHANGER.
    /// </summary>
    public async Task<bool> ChangeStatutAsync(int ticketId, int newStatutId, int agentId)
    {
        var ticket = await _db.Tickets.FindAsync(ticketId);
        if (ticket == null) return false;

        var oldStatutId = ticket.Id_Sta;
        ticket.Id_Sta = newStatutId;

        _db.Changements.Add(new Changer
        {
            Num_Tic      = ticketId,
            Id_Sta_Avant = oldStatutId,
            Id_Sta_Apres = newStatutId,
            Date_Change  = DateTime.UtcNow,
            Id_Agt       = agentId
        });

        await _db.SaveChangesAsync();
        return true;
    }
}
