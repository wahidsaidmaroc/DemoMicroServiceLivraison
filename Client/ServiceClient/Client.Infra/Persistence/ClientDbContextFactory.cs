using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Client.Infra.Persistence;

public class ClientDbContextFactory : IDesignTimeDbContextFactory<ClientDbContext>
{
    public ClientDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ClientDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost\\SQLEXPRESS;Database=ClientDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");

        return new ClientDbContext(optionsBuilder.Options);
    }
}