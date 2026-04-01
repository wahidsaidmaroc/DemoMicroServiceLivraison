using Client.Domain.Enums;
using ClientEntity = global::Client.Domain.Entities.Client;
using Microsoft.EntityFrameworkCore;

namespace Client.Infra.Persistence;

public class ClientDbContext(DbContextOptions<ClientDbContext> options) : DbContext(options)
{
    public DbSet<ClientEntity> Clients => Set<ClientEntity>();

    private static readonly Guid ClientParticulierId = new("7A57D85C-39D6-4F8E-9B74-15937C7E8F10");
    private static readonly Guid ClientEntrepriseId = new("BC16CE57-5E0C-434B-8F23-DC0FBF5AF3A1");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClientEntity>(entity =>
        {
            entity.ToTable("Client");

            entity.HasKey(client => client.Id);

            entity.Property(client => client.Nom)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(client => client.Email)
                .HasMaxLength(200);

            entity.Property(client => client.Telephone)
                .HasMaxLength(30);

            entity.Property(client => client.CIN)
                .HasMaxLength(30);

            entity.Property(client => client.ICE)
                .HasMaxLength(30);

            entity.Property(client => client.TypeClient)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.OwnsOne(client => client.Adresse, address =>
            {
                address.Property(valueObject => valueObject.LigneComplete)
                    .HasColumnName("Adresse")
                    .HasMaxLength(300);

                address.Property(valueObject => valueObject.Ville)
                    .HasColumnName("Ville")
                    .HasMaxLength(100);

                address.Property(valueObject => valueObject.Pays)
                    .HasColumnName("Pays")
                    .HasMaxLength(100);

                address.HasData(
                    new
                    {
                        ClientId = ClientParticulierId,
                        LigneComplete = "12 Avenue Mohammed V",
                        Ville = "Casablanca",
                        Pays = "Maroc"
                    },
                    new
                    {
                        ClientId = ClientEntrepriseId,
                        LigneComplete = "Parc Technopolis, Rocade Rabat-Sale",
                        Ville = "Rabat",
                        Pays = "Maroc"
                    }
                );
            });

            entity.HasData(
                new
                {
                    Id = ClientParticulierId,
                    Nom = "Said WAHID",
                    TypeClient = TypeClient.Particulier,
                    Email = "said.wahid@example.ma",
                    Telephone = "+212612345678",
                    CIN = "BE123456",
                    ICE = string.Empty,
                    DateCreation = new DateTime(2026, 4, 1, 8, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = ClientEntrepriseId,
                    Nom = "Atlas Logistics",
                    TypeClient = TypeClient.Entreprise,
                    Email = "contact@atlas-logistics.ma",
                    Telephone = "+212522334455",
                    CIN = string.Empty,
                    ICE = "001234567000001",
                    DateCreation = new DateTime(2026, 4, 1, 8, 15, 0, DateTimeKind.Utc)
                }
            );
        });

        
    }
}