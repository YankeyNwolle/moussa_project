namespace HelpdeskAPI.Models;

public class Role
{
    public int Id_Rol { get; set; }
    public string Libelle { get; set; } = string.Empty;

    public ICollection<Agent> Agents { get; set; } = new List<Agent>();
}
