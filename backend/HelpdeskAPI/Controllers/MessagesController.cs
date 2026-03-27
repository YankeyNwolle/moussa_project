using System.Security.Claims;
using HelpdeskAPI.Data;
using HelpdeskAPI.DTOs;
using HelpdeskAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskAPI.Controllers;

[ApiController]
[Route("api/tickets/{ticketId}/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public MessagesController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    // GET /api/tickets/{ticketId}/messages
    [HttpGet]
    public async Task<IActionResult> GetMessages(int ticketId)
    {
        var messages = await _db.Messages
            .Include(m => m.Client)
            .Include(m => m.Agent)
            .Where(m => m.Num_Tic == ticketId)
            .OrderBy(m => m.Date_Mes)
            .ToListAsync();

        return Ok(messages.Select(m => new MessageDto(
            m.Cod_Mes,
            m.Cont,
            m.Date_Mes,
            m.Image,
            m.Client != null ? $"{m.Client.Prenom} {m.Client.Nom}"
                             : (m.Agent != null ? $"{m.Agent.Prenom} {m.Agent.Nom}" : "Inconnu"),
            m.Id_Cli.HasValue ? "Client" : "Agent"
        )));
    }

    // POST /api/tickets/{ticketId}/messages  (multipart/form-data)
    [HttpPost]
    public async Task<IActionResult> SendMessage(int ticketId, [FromForm] string cont,
        IFormFile? attachment)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);
        var role   = User.FindFirst(ClaimTypes.Role)!.Value;

        // Ensure ticket exists
        if (!await _db.Tickets.AnyAsync(t => t.Num_Tic == ticketId))
            return NotFound(new { message = "Ticket introuvable." });

        string? imagePath = null;
        if (attachment != null && attachment.Length > 0)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(attachment.FileName)}";
            var fullPath = Path.Combine(uploadsDir, fileName);
            await using var stream = System.IO.File.Create(fullPath);
            await attachment.CopyToAsync(stream);
            imagePath = $"/uploads/{fileName}";
        }

        var message = new Message
        {
            Cont     = cont,
            Date_Mes = DateTime.UtcNow,
            Image    = imagePath,
            Num_Tic  = ticketId,
            Id_Cli   = role == "Client" ? userId : null,
            Id_Agt   = role != "Client"  ? userId : null,
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync();

        return Ok(new { message.Cod_Mes, imagePath });
    }
}
