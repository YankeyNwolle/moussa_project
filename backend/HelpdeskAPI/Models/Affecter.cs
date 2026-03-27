namespace HelpdeskAPI.Models;

public class Affecter
{
    public int Num_Tic { get; set; }
    public int Id_Agt { get; set; }
    public DateTime Date_Affectation { get; set; } = DateTime.UtcNow;

    public Ticket? Ticket { get; set; }
    public Agent? Agent { get; set; }
}
