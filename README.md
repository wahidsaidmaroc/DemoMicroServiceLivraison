# 🎓 TP : Microservice Facture – Clean Architecture

---

## 1️⃣ Structure Clean Architecture

```
FactureService/
│
├── FactureService.API             # Point d'entrée : Controllers, Swagger
├── FactureService.Application     # CQRS : Commands, Queries, Handlers, DTOs, Validators
├── FactureService.Domain          # Entités métier
├── FactureService.Infrastructure  # DbContext, Configurations Fluent API, Persistence
```

### Règles

- **Domain** : contient uniquement la logique métier (entités, valeurs objets)
- **Application** : logique métier orchestrée (CQRS, Handlers, validation)
- **Infrastructure** : accès aux données, EF Core, Fluent API
- **API** : Controllers et point d'entrée

---

## 2️⃣ Domain Layer

### `Facture.cs`

```csharp
namespace FactureService.Domain.Entities
{
    // Entité Facture
    public class Facture
    {
        public Guid Id { get; set; }
        public string Numero { get; set; }
        public DateTime DateFacture { get; set; }
        public decimal Total { get; set; }

        // Liste des lignes
        public List<LigneFacture> Lignes { get; set; } = new();
    }
}
```

### `LigneFacture.cs`

```csharp
namespace FactureService.Domain.Entities
{
    public class LigneFacture
    {
        public Guid Id { get; set; }
        public string Produit { get; set; }
        public int Quantite { get; set; }
        public decimal Prix { get; set; }

        public Guid FactureId { get; set; }
        public Facture Facture { get; set; }
    }
}
```

---

## 3️⃣ Infrastructure Layer

### `FactureDbContext.cs`

```csharp
using FactureService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FactureService.Infrastructure.Data
{
    public class FactureDbContext : DbContext
    {
        public FactureDbContext(DbContextOptions<FactureDbContext> options) : base(options) { }

        public DbSet<Facture> Factures { get; set; }
        public DbSet<LigneFacture> LignesFactures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Charger toutes les configurations Fluent API automatiquement
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FactureDbContext).Assembly);
        }
    }
}
```

### `FactureConfiguration.cs`

```csharp
using FactureService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FactureService.Infrastructure.Configurations
{
    public class FactureConfiguration : IEntityTypeConfiguration<Facture>
    {
        public void Configure(EntityTypeBuilder<Facture> builder)
        {
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Numero).IsRequired().HasMaxLength(50);
            builder.Property(f => f.Total).HasColumnType("decimal(18,2)");

            builder.HasMany(f => f.Lignes)
                   .WithOne(l => l.Facture)
                   .HasForeignKey(l => l.FactureId);
        }
    }
}
```

---

## 4️⃣ Application Layer – CQRS + FluentValidation

### `DTOs/CreateFactureDto.cs`

```csharp
namespace FactureService.Application.DTOs
{
    public class CreateFactureDto
    {
        public string Numero { get; set; }
        public DateTime DateFacture { get; set; }
        public List<CreateLigneFactureDto> Lignes { get; set; }
    }

    public class CreateLigneFactureDto
    {
        public string Produit { get; set; }
        public int Quantite { get; set; }
        public decimal Prix { get; set; }
    }
}
```

### `Validators/FactureValidator.cs`

```csharp
using FluentValidation;
using FactureService.Application.DTOs;

namespace FactureService.Application.Validators
{
    public class FactureValidator : AbstractValidator<CreateFactureDto>
    {
        public FactureValidator()
        {
            RuleFor(f => f.Numero)
                .NotEmpty()
                .WithMessage("Le numéro de facture est obligatoire");

            RuleFor(f => f.Lignes)
                .NotEmpty()
                .WithMessage("La facture doit avoir au moins une ligne");

            RuleForEach(f => f.Lignes).ChildRules(lignes =>
            {
                lignes.RuleFor(l => l.Quantite)
                    .GreaterThan(0)
                    .WithMessage("La quantité doit être > 0");

                lignes.RuleFor(l => l.Prix)
                    .GreaterThan(0)
                    .WithMessage("Le prix doit être > 0");
            });
        }
    }
}
```

### `Commands/CreateFactureCommand.cs`

```csharp
using FactureService.Application.DTOs;

namespace FactureService.Application.Commands
{
    public class CreateFactureCommand
    {
        public CreateFactureDto Facture { get; set; }
    }
}
```

### `Handlers/CreateFactureHandler.cs`

```csharp
using FactureService.Application.Commands;
using FactureService.Domain.Entities;
using FactureService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FactureService.Application.Handlers
{
    public class CreateFactureHandler
    {
        private readonly FactureDbContext _context;

        public CreateFactureHandler(FactureDbContext context) => _context = context;

        public async Task<Guid> Handle(CreateFactureCommand command)
        {
            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                Numero = command.Facture.Numero,
                DateFacture = command.Facture.DateFacture,
                Lignes = command.Facture.Lignes.Select(l => new LigneFacture
                {
                    Id = Guid.NewGuid(),
                    Produit = l.Produit,
                    Quantite = l.Quantite,
                    Prix = l.Prix
                }).ToList()
            };

            facture.Total = facture.Lignes.Sum(l => l.Quantite * l.Prix);

            _context.Factures.Add(facture);
            await _context.SaveChangesAsync();
            return facture.Id;
        }
    }
}
```

---

## 5️⃣ API Layer – Controllers

### `FacturesController.cs`

```csharp
using FactureService.Application.Commands;
using FactureService.Application.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace FactureService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturesController : ControllerBase
    {
        private readonly CreateFactureHandler _createHandler;

        public FacturesController(CreateFactureHandler createHandler)
        {
            _createHandler = createHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateFactureCommand command)
        {
            var id = await _createHandler.Handle(command);
            return Ok(id); // Retourne l'ID de la facture créée
        }

        // TODO : Ajouter Get / Update / Delete respectant CQRS
    }
}
```

---

## 6️⃣ Instructions pour les étudiants

1. Créer le projet **FactureService** dans Visual Studio
2. Ajouter les projets **API**, **Application**, **Domain**, **Infrastructure**
3. Installer les packages **EF Core** + **FluentValidation**
4. Ajouter toutes les entités et configurations **Fluent API**
5. Implémenter **CQRS** et **Validators**
6. Créer les **Controllers** pour exposer les endpoints
7. Tester avec **Swagger** ou **Postman**
