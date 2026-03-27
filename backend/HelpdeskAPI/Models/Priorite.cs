namespace HelpdeskAPI.Models;

public class Priorite
{
    public int Id_Pri { get; set; }
    public string Libelle { get; set; } = string.Empty;
    public int Niveau { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
