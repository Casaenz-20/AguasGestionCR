using System;
using System.Collections.Generic;

namespace AguasGestionCR.Models;

public partial class Cliente
{
    public int ClienteId { get; set; }

    public string? CodigoCliente { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string Identificacion { get; set; } = null!;

    public string EstadoPrevista { get; set; } = null!;

    public string? NumeroMedidor { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? CorreoElectronico { get; set; }

    public string Estado { get; set; } = null!;

    public byte[]? DocumentoCedulaPdf { get; set; }

    public string? NombreArchivoCedula { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime UltimaActualizacion { get; set; }
}
