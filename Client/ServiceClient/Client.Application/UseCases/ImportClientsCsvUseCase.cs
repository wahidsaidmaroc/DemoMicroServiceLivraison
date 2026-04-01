using Client.Application.DTOs;
using Client.Application.Interfaces;
using Client.Domain.Enums;
using Client.Domain.ValueObjects;
using ClientEntity = Client.Domain.Entities.Client;

namespace Client.Application.UseCases;

public class ImportClientsCsvUseCase(IClientRepository repository)
{
    private static readonly HashSet<string> VillesAutorisees =
        new(StringComparer.OrdinalIgnoreCase) { "Casa", "Rabat", "Fes" };

    public async Task<ImportCsvResultDTO> ExecuteAsync(Stream csvStream, CancellationToken cancellationToken = default)
    {
        var result = new ImportCsvResultDTO();
        var clientsToInsert = new List<ClientEntity>();

        using var reader = new StreamReader(csvStream);

        // Ignorer la ligne d'en-tête
        var header = await reader.ReadLineAsync(cancellationToken);
        if (header is null)
        {
            result.Erreurs.Add(new ImportCsvErreurDTO
            {
                Ligne = 0,
                Cause = ["Le fichier CSV est vide ou ne contient pas d'en-tête."]
            });
            result.Rejetes = 0;
            return result;
        }

        int lineNumber = 1;
        string? line;

        while ((line = await reader.ReadLineAsync(cancellationToken)) is not null)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = line.Split(',');

            if (columns.Length < 9)
            {
                result.Erreurs.Add(new ImportCsvErreurDTO
                {
                    Ligne = lineNumber,
                    Cause = [$"Ligne malformée : {columns.Length} colonne(s) trouvée(s), 9 attendues."]
                });
                continue;
            }

            var nom       = columns[0].Trim();
            var typeStr   = columns[1].Trim();
            var email     = columns[2].Trim();
            var telephone = columns[3].Trim();
            var adresse   = columns[4].Trim();
            var ville     = columns[5].Trim();
            var pays      = columns[6].Trim();
            var cin       = columns[7].Trim();
            var ice       = columns[8].Trim();

            var causes = new List<string>();

            // Règle métier : Nom minimum 4 caractères
            if (nom.Length < 4)
                causes.Add($"Le champ Nom '{nom}' doit comporter au minimum 4 caractères.");

            // Règle métier : Ville autorisée
            if (!VillesAutorisees.Contains(ville))
                causes.Add($"La ville '{ville}' n'est pas autorisée. Valeurs acceptées : Casa, Rabat, Fes.");

            // Règle métier : TypeClient valide
            if (!Enum.TryParse<TypeClient>(typeStr, ignoreCase: true, out var typeClient))
                causes.Add($"Le type client '{typeStr}' est invalide. Valeurs acceptées : Particulier, ONG, Entreprise.");

            if (causes.Count > 0)
            {
                result.Erreurs.Add(new ImportCsvErreurDTO
                {
                    Ligne = lineNumber,
                    Nom   = nom,
                    Cause = causes
                });
                continue;
            }

            clientsToInsert.Add(new ClientEntity
            {
                Id           = Guid.NewGuid(),
                Nom          = nom,
                TypeClient   = typeClient,
                Email        = email,
                Telephone    = telephone,
                CIN          = cin,
                ICE          = ice,
                DateCreation = DateTime.UtcNow,
                Adresse      = new Adresse
                {
                    LigneComplete = adresse,
                    Ville         = ville,
                    Pays          = pays
                }
            });
        }

        if (clientsToInsert.Count > 0)
            await repository.AddRangeAsync(clientsToInsert, cancellationToken);

        result.Inseres = clientsToInsert.Count;
        result.Rejetes = result.Erreurs.Count;

        return result;
    }
}
