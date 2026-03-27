namespace HelpdeskAPI.Models;

public class Statut
{
    public int Id_Sta { get; set; }
    public string Libelle { get; set; } = string.Empty;

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Changer> ChangementsAvant { get; set; } = new List<Changer>();
    public ICollection<Changer> ChangementsApres { get; set; } = new List<Changer>();
}
