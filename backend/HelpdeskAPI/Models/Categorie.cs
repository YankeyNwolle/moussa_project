namespace HelpdeskAPI.Models;

public class Categorie
{
    public int Cod_Cat { get; set; }
    public string Libelle { get; set; } = string.Empty;
    public int Ref_Equ { get; set; }

    public Equipe? Equipe { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
