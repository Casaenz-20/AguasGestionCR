using AguasGestionCR.Helpers;
using AguasGestionCR.Interfaces;
using AguasGestionCR.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace AguasGestionCR.Services
{
    internal class ClienteServices : IClientes
    {
        private readonly AcueductoDbContext _context;

        // Inyección de dependencias para evitar crear y destruir contextos manualmente
        public ClienteServices(AcueductoDbContext context)
        {
            _context = context;
        }

        public int RegistrarCliente(Cliente cliente)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            if (string.IsNullOrWhiteSpace(cliente.Identificacion)) throw new ArgumentException("La identificación es obligatoria.");

            // 1. ASIGNAR MEDIDOR AUTOMÁTICAMENTE SI VIENE VACÍO
            if (string.IsNullOrWhiteSpace(cliente.NumeroMedidor))
            {
                // Genera un número de medidor automático único, ej: MED-2026-8492
                string randomNum = new Random().Next(1000, 9999).ToString();
                cliente.NumeroMedidor = $"MED-{DateTime.Now.Year}-{randomNum}";
            }

            if (cliente.FechaRegistro == default) cliente.FechaRegistro = DateTime.Now;
            cliente.UltimaActualizacion = DateTime.Now;

            var conn = _context.Database.GetDbConnection();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "RegistrarCliente";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(DbHelper.CrearParametro(cmd, "@NombreCompleto", cliente.NombreCompleto));
                cmd.Parameters.Add(DbHelper.CrearParametro(cmd, "@Identificacion", cliente.Identificacion));
                cmd.Parameters.Add(DbHelper.CrearParametro(cmd, "@EstadoPrevista", cliente.EstadoPrevista));
                // Enviamos el medidor generado automáticamente
                cmd.Parameters.Add(DbHelper.CrearParametro(cmd, "@NumeroMedidor", cliente.NumeroMedidor));
                cmd.Parameters.Add(DbHelper.CrearParametro(cmd, "@Direccion", (object?)cliente.Direccion ?? DBNull.Value));
                cmd.Parameters.Add(DbHelper.CrearParametro(cmd, "@Telefono", (object?)cliente.Telefono ?? DBNull.Value));
                cmd.Parameters.Add(DbHelper.CrearParametro(cmd, "@CorreoElectronico", (object?)cliente.CorreoElectronico ?? DBNull.Value));
                cmd.Parameters.Add(DbHelper.CrearParametro(cmd, "@FechaRegistro", cliente.FechaRegistro));
                cmd.Parameters.Add(DbHelper.CrearParametro(cmd, "@DocumentoCedulaPdf", (object?)cliente.DocumentoCedulaPdf ?? DBNull.Value));
                cmd.Parameters.Add(DbHelper.CrearParametro(cmd, "@NombreArchivoCedula", (object?)cliente.NombreArchivoCedula ?? DBNull.Value));

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                var id = cmd.ExecuteScalar();
                return Convert.ToInt32(id);
            }
        }

        public Cliente ObtenerClientePorId(int id)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.ClienteId == id);

            if (cliente == null)
                throw new Exception("Cliente no encontrado");

            return cliente;
        }

        public bool EditarCliente(Cliente cliente)
        {
            if (cliente == null || cliente.ClienteId <= 0)
                throw new ArgumentException("Datos de cliente inválidos para actualizar.");

            var clienteExistente = _context.Clientes.Find(cliente.ClienteId);
            if (clienteExistente == null)
                throw new Exception("El cliente que intenta editar no existe.");

            clienteExistente.NombreCompleto = cliente.NombreCompleto;
            clienteExistente.Identificacion = cliente.Identificacion;
            clienteExistente.EstadoPrevista = cliente.EstadoPrevista;
            clienteExistente.NumeroMedidor = cliente.NumeroMedidor;
            clienteExistente.Direccion = cliente.Direccion;
            clienteExistente.Telefono = cliente.Telefono;
            clienteExistente.CorreoElectronico = cliente.CorreoElectronico;
            clienteExistente.Estado = cliente.Estado;
            clienteExistente.UltimaActualizacion = DateTime.Now;

            // Por si se actualiza el documentyo de la cedula,se reemplaza el archivo 
            if (cliente.DocumentoCedulaPdf != null)
            {
                clienteExistente.DocumentoCedulaPdf = cliente.DocumentoCedulaPdf;
                clienteExistente.NombreArchivoCedula = cliente.NombreArchivoCedula;
            }

            int filasAfectadas = _context.SaveChanges();
            return filasAfectadas > 0;
        }

        public string EliminarCliente(int id)
        {
            var cliente = _context.Clientes
                                  .FirstOrDefault(c => c.ClienteId == id && c.Estado == "Activo");

            if (cliente == null)
                throw new Exception("Cliente no encontrado o ya se encuentra inactivo.");

            // Cambiamos el estado del cliente a "Inactivo"
            cliente.Estado = "Inactivo";
            cliente.UltimaActualizacion = DateTime.Now;

            // 3. Indicamos a Entity Framework que actualice el registro y guardamos
            _context.Clientes.Update(cliente);
            _context.SaveChanges();

            return "Cliente inactivo";
        }
    }
}