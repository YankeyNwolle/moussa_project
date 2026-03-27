namespace HelpdeskAPI.Models;

public class Agent
{
    public int Id_Agt { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Mdp { get; set; } = string.Empty;  // BCrypt hash
    public int Ref_Equ { get; set; }
    public int Id_Rol { get; set; }

    public Equipe? Equipe { get; set; }
    public Role? Role { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Affecter> Affectations { get; set; } = new List<Affecter>();
    public ICollection<Changer> Changements { get; set; } = new List<Changer>();
}
