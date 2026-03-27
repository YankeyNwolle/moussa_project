namespace HelpdeskAPI.Models;

public class Changer
{
    public int Id_Cha { get; set; }
    public int Num_Tic { get; set; }
    public int? Id_Sta_Avant { get; set; }
    public int Id_Sta_Apres { get; set; }
    public DateTime Date_Change { get; set; } = DateTime.UtcNow;
    public int? Id_Agt { get; set; }

    public Ticket? Ticket { get; set; }
    public Statut? StatutAvant { get; set; }
    public Statut? StatutApres { get; set; }
    public Agent? Agent { get; set; }
}
