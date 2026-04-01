using Client.Domain.Enums;
using Client.Domain.ValueObjects;

namespace Client.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public TypeClient TypeClient { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Telephone { get; set; } = string.Empty;

    public Adresse Adresse { get; set; } = new();

    public string CIN { get; set; } = string.Empty;

    public string ICE { get; set; } = string.Empty;

    public DateTime DateCreation { get; set; }
}