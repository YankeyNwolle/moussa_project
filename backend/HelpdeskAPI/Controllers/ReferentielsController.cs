using HelpdeskAPI.Data;
using HelpdeskAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskAPI.Controllers;

/// <summary>
/// Public referentiels: categories and priorities (used by the client when creating a ticket)
/// </summary>
[ApiController]
[Route("api/referentiels")]
public class ReferentielsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ReferentielsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(new
        {
            categories = await _db.Categories
                .Include(c => c.Equipe)
                .Select(c => new { c.Cod_Cat, c.Libelle, equipe = c.Equipe!.Libelle })
                .ToListAsync(),
            priorites = await _db.Priorites
                .OrderBy(p => p.Niveau)
                .Select(p => new { p.Id_Pri, p.Libelle, p.Niveau })
                .ToListAsync(),
        });
    }
}
