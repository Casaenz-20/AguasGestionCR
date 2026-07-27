using System;
using System.Collections.Generic;

namespace AguasGestionCR.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string Identificacion { get; set; } = null!;

    public string? NumeroMedidor { get; set; }

    public string? CorreoElectronico { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string ContrasenaHash { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public bool Estado { get; set; }

    public DateTime FechaCreacion { get; set; }
}
