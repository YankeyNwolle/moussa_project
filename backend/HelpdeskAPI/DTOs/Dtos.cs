namespace HelpdeskAPI.DTOs;

// ---------- Auth ----------
public record LoginRequest(string Login, string Mdp);

public record RegisterClientRequest(
    string Nom,
    string Prenom,
    string Email,
    string? Tel,
    string Login,
    string Mdp
);

public record AuthResponse(
    string Token,
    string Role,       // "Client" | "Agent"
    int UserId,
    string NomComplet
);

// ---------- Ticket ----------
public record CreateTicketRequest(
    string Titre,
    string Descr,
    int Id_Pri,
    int Cod_Cat
);

public record UpdateTicketStatusRequest(int Id_Sta_Apres);

public record TicketDto(
    int Num_Tic,
    string Titre,
    string Descr,
    DateTime Datecre,
    string StatutLibelle,
    string PrioriteLibelle,
    string CategorieLibelle,
    string EquipeLibelle,
    string ClientNom,
    Agent? AgentAssigne
);

public record Agent(int Id_Agt, string NomComplet);

// ---------- Message ----------
public record CreateMessageRequest(string Cont);

public record MessageDto(
    int Cod_Mes,
    string Cont,
    DateTime Date_Mes,
    string? Image,
    string? AuteurNom,   // Nom de l'expéditeur
    string AuteurRole    // "Client" | "Agent"
);

// ---------- History ----------
public record ChangerDto(
    int Id_Cha,
    string? StatutAvant,
    string StatutApres,
    DateTime Date_Change,
    string? AgentNom
);
