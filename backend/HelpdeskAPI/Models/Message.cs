namespace HelpdeskAPI.Models;

public class Message
{
    public int Cod_Mes { get; set; }
    public string Cont { get; set; } = string.Empty;
    public DateTime Date_Mes { get; set; } = DateTime.UtcNow;
    public string? Image { get; set; }   // relative file path
    public int Num_Tic { get; set; }
    public int? Id_Cli { get; set; }    // null if sent by agent
    public int? Id_Agt { get; set; }    // null if sent by client

    public Ticket? Ticket { get; set; }
    public Client? Client { get; set; }
    public Agent? Agent { get; set; }
}
