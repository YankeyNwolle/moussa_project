namespace HelpdeskAPI.Models;

public class Client
{
    public int Id_Cli { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Tel { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Mdp { get; set; } = string.Empty;  // BCrypt hash

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
