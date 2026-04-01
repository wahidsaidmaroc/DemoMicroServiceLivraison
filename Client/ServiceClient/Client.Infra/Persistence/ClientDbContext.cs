using Client.Domain.Enums;
using ClientEntity = global::Client.Domain.Entities.Client;
using Microsoft.EntityFrameworkCore;

namespace Client.Infra.Persistence;

public class ClientDbContext(DbContextOptions<ClientDbContext> options) : DbContext(options)
{
    public DbSet<ClientEntity> Clients => Set<ClientEntity>();


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
            });
        });
    }
}