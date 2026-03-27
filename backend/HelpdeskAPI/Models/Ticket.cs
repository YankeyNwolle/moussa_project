namespace HelpdeskAPI.Models;

public class Ticket
{
    public int Num_Tic { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Descr { get; set; } = string.Empty;
    public DateTime Datecre { get; set; } = DateTime.UtcNow;
    public int Id_Cli { get; set; }
    public int Cod_Cat { get; set; }
    public int Id_Pri { get; set; }
    public int Id_Sta { get; set; }

    public Client? Client { get; set; }
    public Categorie? Categorie { get; set; }
    public Priorite? Priorite { get; set; }
    public Statut? Statut { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Affecter> Affectations { get; set; } = new List<Affecter>();
    public ICollection<Changer> Historique { get; set; } = new List<Changer>();
}
