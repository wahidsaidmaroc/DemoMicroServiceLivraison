using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Client.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TypeClient = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Adresse = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Ville = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pays = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CIN = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ICE = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Client",
                columns: new[] { "Id", "CIN", "DateCreation", "Email", "ICE", "Nom", "Telephone", "TypeClient", "Adresse", "Pays", "Ville" },
                values: new object[,]
                {
                    { new Guid("7a57d85c-39d6-4f8e-9b74-15937c7e8f10"), "BE123456", new DateTime(2026, 4, 1, 8, 0, 0, 0, DateTimeKind.Utc), "said.wahid@example.ma", "", "Said WAHID", "+212612345678", "Particulier", "12 Avenue Mohammed V", "Maroc", "Casablanca" },
                    { new Guid("bc16ce57-5e0c-434b-8f23-dc0fbf5af3a1"), "", new DateTime(2026, 4, 1, 8, 15, 0, 0, DateTimeKind.Utc), "contact@atlas-logistics.ma", "001234567000001", "Atlas Logistics", "+212522334455", "Entreprise", "Parc Technopolis, Rocade Rabat-Sale", "Maroc", "Rabat" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Client");
        }
    }
}
