using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Application.DTOs;

public class ClientDTO
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
}

public class ClientCreateDTO
{
    public string Nom { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string TypeClient { get; set; } = string.Empty;
}

public class ImportCsvResultDTO
{
    public int Inseres { get; set; }
    public int Rejetes { get; set; }
    public List<ImportCsvErreurDTO> Erreurs { get; set; } = [];
}

public class ImportCsvErreurDTO
{
    public int Ligne { get; set; }
    public string? Nom { get; set; }
    public List<string> Cause { get; set; } = [];
}
