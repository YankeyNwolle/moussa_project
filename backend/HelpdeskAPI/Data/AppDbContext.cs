using Microsoft.EntityFrameworkCore;
using HelpdeskAPI.Models;

namespace HelpdeskAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role> Roles { get; set; }
    public DbSet<Equipe> Equipes { get; set; }
    public DbSet<Priorite> Priorites { get; set; }
    public DbSet<Statut> Statuts { get; set; }
    public DbSet<Categorie> Categories { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Affecter> Affectations { get; set; }
    public DbSet<Changer> Changements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Table names (match SQL script) ----------
        modelBuilder.Entity<Role>().ToTable("ROLE");
        modelBuilder.Entity<Equipe>().ToTable("EQUIPE");
        modelBuilder.Entity<Priorite>().ToTable("PRIORITE");
        modelBuilder.Entity<Statut>().ToTable("STATUT");
        modelBuilder.Entity<Categorie>().ToTable("CATEGORIE");
        modelBuilder.Entity<Client>().ToTable("CLIENT");
        modelBuilder.Entity<Agent>().ToTable("AGENT");
        modelBuilder.Entity<Ticket>().ToTable("TICKET");
        modelBuilder.Entity<Message>().ToTable("MESSAGE");
        modelBuilder.Entity<Affecter>().ToTable("AFFECTER");
        modelBuilder.Entity<Changer>().ToTable("CHANGER");

        // ---------- PKs ----------
        modelBuilder.Entity<Role>().HasKey(r => r.Id_Rol);
        modelBuilder.Entity<Equipe>().HasKey(e => e.Ref_Equ);
        modelBuilder.Entity<Priorite>().HasKey(p => p.Id_Pri);
        modelBuilder.Entity<Statut>().HasKey(s => s.Id_Sta);
        modelBuilder.Entity<Categorie>().HasKey(c => c.Cod_Cat);
        modelBuilder.Entity<Client>().HasKey(c => c.Id_Cli);
        modelBuilder.Entity<Agent>().HasKey(a => a.Id_Agt);
        modelBuilder.Entity<Ticket>().HasKey(t => t.Num_Tic);
        modelBuilder.Entity<Message>().HasKey(m => m.Cod_Mes);
        modelBuilder.Entity<Changer>().HasKey(c => c.Id_Cha);

        // Composite PK for AFFECTER
        modelBuilder.Entity<Affecter>().HasKey(a => new { a.Num_Tic, a.Id_Agt });

        // ---------- Column mappings ----------
        modelBuilder.Entity<Role>().Property(r => r.Id_Rol).HasColumnName("id_Rol");
        modelBuilder.Entity<Role>().Property(r => r.Libelle).HasColumnName("libelle");

        modelBuilder.Entity<Equipe>().Property(e => e.Ref_Equ).HasColumnName("ref_Equ");
        modelBuilder.Entity<Equipe>().Property(e => e.Libelle).HasColumnName("libelle");

        modelBuilder.Entity<Priorite>().Property(p => p.Id_Pri).HasColumnName("id_Pri");
        modelBuilder.Entity<Priorite>().Property(p => p.Libelle).HasColumnName("libelle");
        modelBuilder.Entity<Priorite>().Property(p => p.Niveau).HasColumnName("niveau");

        modelBuilder.Entity<Statut>().Property(s => s.Id_Sta).HasColumnName("id_Sta");
        modelBuilder.Entity<Statut>().Property(s => s.Libelle).HasColumnName("libelle");

        modelBuilder.Entity<Categorie>().Property(c => c.Cod_Cat).HasColumnName("cod_Cat");
        modelBuilder.Entity<Categorie>().Property(c => c.Libelle).HasColumnName("libelle");
        modelBuilder.Entity<Categorie>().Property(c => c.Ref_Equ).HasColumnName("ref_Equ");

        modelBuilder.Entity<Client>().Property(c => c.Id_Cli).HasColumnName("id_Cli");
        modelBuilder.Entity<Client>().Property(c => c.Nom).HasColumnName("nom");
        modelBuilder.Entity<Client>().Property(c => c.Prenom).HasColumnName("prenom");
        modelBuilder.Entity<Client>().Property(c => c.Email).HasColumnName("email");
        modelBuilder.Entity<Client>().Property(c => c.Tel).HasColumnName("tel");
        modelBuilder.Entity<Client>().Property(c => c.Login).HasColumnName("login");
        modelBuilder.Entity<Client>().Property(c => c.Mdp).HasColumnName("mdp");

        modelBuilder.Entity<Agent>().Property(a => a.Id_Agt).HasColumnName("id_Agt");
        modelBuilder.Entity<Agent>().Property(a => a.Nom).HasColumnName("nom");
        modelBuilder.Entity<Agent>().Property(a => a.Prenom).HasColumnName("prenom");
        modelBuilder.Entity<Agent>().Property(a => a.Email).HasColumnName("email");
        modelBuilder.Entity<Agent>().Property(a => a.Login).HasColumnName("login");
        modelBuilder.Entity<Agent>().Property(a => a.Mdp).HasColumnName("mdp");
        modelBuilder.Entity<Agent>().Property(a => a.Ref_Equ).HasColumnName("ref_Equ");
        modelBuilder.Entity<Agent>().Property(a => a.Id_Rol).HasColumnName("id_Rol");

        modelBuilder.Entity<Ticket>().Property(t => t.Num_Tic).HasColumnName("num_Tic");
        modelBuilder.Entity<Ticket>().Property(t => t.Titre).HasColumnName("titre");
        modelBuilder.Entity<Ticket>().Property(t => t.Descr).HasColumnName("descr");
        modelBuilder.Entity<Ticket>().Property(t => t.Datecre).HasColumnName("datecre");
        modelBuilder.Entity<Ticket>().Property(t => t.Id_Cli).HasColumnName("id_Cli");
        modelBuilder.Entity<Ticket>().Property(t => t.Cod_Cat).HasColumnName("cod_Cat");
        modelBuilder.Entity<Ticket>().Property(t => t.Id_Pri).HasColumnName("id_Pri");
        modelBuilder.Entity<Ticket>().Property(t => t.Id_Sta).HasColumnName("id_Sta");

        modelBuilder.Entity<Message>().Property(m => m.Cod_Mes).HasColumnName("cod_Mes");
        modelBuilder.Entity<Message>().Property(m => m.Cont).HasColumnName("cont");
        modelBuilder.Entity<Message>().Property(m => m.Date_Mes).HasColumnName("date_Mes");
        modelBuilder.Entity<Message>().Property(m => m.Image).HasColumnName("image");
        modelBuilder.Entity<Message>().Property(m => m.Num_Tic).HasColumnName("num_Tic");
        modelBuilder.Entity<Message>().Property(m => m.Id_Cli).HasColumnName("id_Cli");
        modelBuilder.Entity<Message>().Property(m => m.Id_Agt).HasColumnName("id_Agt");

        modelBuilder.Entity<Affecter>().Property(a => a.Num_Tic).HasColumnName("num_Tic");
        modelBuilder.Entity<Affecter>().Property(a => a.Id_Agt).HasColumnName("id_Agt");
        modelBuilder.Entity<Affecter>().Property(a => a.Date_Affectation).HasColumnName("date_affectation");

        modelBuilder.Entity<Changer>().Property(c => c.Id_Cha).HasColumnName("id_Cha");
        modelBuilder.Entity<Changer>().Property(c => c.Num_Tic).HasColumnName("num_Tic");
        modelBuilder.Entity<Changer>().Property(c => c.Id_Sta_Avant).HasColumnName("id_Sta_avant");
        modelBuilder.Entity<Changer>().Property(c => c.Id_Sta_Apres).HasColumnName("id_Sta_apres");
        modelBuilder.Entity<Changer>().Property(c => c.Date_Change).HasColumnName("date_change");
        modelBuilder.Entity<Changer>().Property(c => c.Id_Agt).HasColumnName("id_Agt");

        // ---------- Relationships ----------

        // Equipe → Agents
        modelBuilder.Entity<Agent>()
            .HasOne(a => a.Equipe)
            .WithMany(e => e.Agents)
            .HasForeignKey(a => a.Ref_Equ);

        // Role → Agents
        modelBuilder.Entity<Agent>()
            .HasOne(a => a.Role)
            .WithMany(r => r.Agents)
            .HasForeignKey(a => a.Id_Rol);

        // Equipe → Categories
        modelBuilder.Entity<Categorie>()
            .HasOne(c => c.Equipe)
            .WithMany(e => e.Categories)
            .HasForeignKey(c => c.Ref_Equ);

        // Ticket → Client
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Client)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.Id_Cli);

        // Ticket → Categorie
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Categorie)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.Cod_Cat);

        // Ticket → Priorite
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Priorite)
            .WithMany(p => p.Tickets)
            .HasForeignKey(t => t.Id_Pri);

        // Ticket → Statut
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Statut)
            .WithMany(s => s.Tickets)
            .HasForeignKey(t => t.Id_Sta);

        // Message → Ticket
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Ticket)
            .WithMany(t => t.Messages)
            .HasForeignKey(m => m.Num_Tic);

        // Message → Client (optional)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Client)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.Id_Cli)
            .IsRequired(false);

        // Message → Agent (optional)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Agent)
            .WithMany(a => a.Messages)
            .HasForeignKey(m => m.Id_Agt)
            .IsRequired(false);

        // Affecter → Ticket
        modelBuilder.Entity<Affecter>()
            .HasOne(af => af.Ticket)
            .WithMany(t => t.Affectations)
            .HasForeignKey(af => af.Num_Tic);

        // Affecter → Agent
        modelBuilder.Entity<Affecter>()
            .HasOne(af => af.Agent)
            .WithMany(a => a.Affectations)
            .HasForeignKey(af => af.Id_Agt);

        // Changer → Ticket
        modelBuilder.Entity<Changer>()
            .HasOne(ch => ch.Ticket)
            .WithMany(t => t.Historique)
            .HasForeignKey(ch => ch.Num_Tic);

        // Changer → StatutAvant
        modelBuilder.Entity<Changer>()
            .HasOne(ch => ch.StatutAvant)
            .WithMany(s => s.ChangementsAvant)
            .HasForeignKey(ch => ch.Id_Sta_Avant)
            .IsRequired(false);

        // Changer → StatutApres
        modelBuilder.Entity<Changer>()
            .HasOne(ch => ch.StatutApres)
            .WithMany(s => s.ChangementsApres)
            .HasForeignKey(ch => ch.Id_Sta_Apres);

        // Changer → Agent (optional)
        modelBuilder.Entity<Changer>()
            .HasOne(ch => ch.Agent)
            .WithMany(a => a.Changements)
            .HasForeignKey(ch => ch.Id_Agt)
            .IsRequired(false);
    }
}
