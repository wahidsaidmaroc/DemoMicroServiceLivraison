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
