namespace HelpdeskAPI.Models;

public class Equipe
{
    public int Ref_Equ { get; set; }
    public string Libelle { get; set; } = string.Empty;

    public ICollection<Agent> Agents { get; set; } = new List<Agent>();
    public ICollection<Categorie> Categories { get; set; } = new List<Categorie>();
}
