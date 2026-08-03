using AguasGestionCR.Helpers;
using AguasGestionCR.Interfaces;
using AguasGestionCR.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AguasGestionCR.Services
{
    internal class ClienteServices : IClientes
    {
        private readonly AcueductoDbContext _context;
        private readonly EviarReporte _servicioCorreo = new EviarReporte(); // Instancia de la clase de correo de tu compañero

        // Inyección de dependencias para evitar crear y destruir contextos manualmente
        public ClienteServices(AcueductoDbContext context)
        {
            _context = context;
        }

        public int RegistrarCliente(Cliente cliente)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            if (string.IsNullOrWhiteSpace(cliente.Identificacion)) throw new ArgumentException("La identificación es obligatoria.");

            if (string.IsNullOrWhiteSpace(cliente.NumeroMedidor))
            {
                // Genera un número de medidor automático único, ej: MED-2026-8492
                string randomNum = new Random().Next(1000, 9999).ToString();
                cliente.NumeroMedidor = $"MED-{DateTime.Now.Year}-{randomNum}";
            }

            if (cliente.FechaRegistro == default) cliente.FechaRegistro = DateTime.Now;
            cliente.UltimaActualizacion = DateTime.Now;

            // Guardar directamente con Entity Framework 
            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            // Enviar el correo automáticamente al registrar con éxito (si tiene correo registrado)
            if (!string.IsNullOrWhiteSpace(cliente.CorreoElectronico))
            {
                try
                {
                    _servicioCorreo.EnviarCorreoMedidor(
                        cliente.CorreoElectronico,
                        cliente.NombreCompleto,
                        cliente.NumeroMedidor
                    );
                }
                catch (Exception ex)
                {
                    // Si el correo falla por falta de internet u otro motivo, no detenemos el registro pero queda la excepción controlada
                    Console.WriteLine($"Error al enviar correo automático: {ex.Message}");
                }
            }

            return cliente.ClienteId;
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

            // Por si se actualiza el documento de la cédula, se reemplaza el archivo 
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

            // Indicamos a Entity Framework que actualice el registro y guardamos
            _context.Clientes.Update(cliente);
            _context.SaveChanges();

            return "Cliente inactivo";
        }
        public IEnumerable<Cliente> ObtenerClientes(string nombre = null, string identificacion = null, string medidor = null, string estado = "Todos")
        {
            // la consulta base de la tabla Clientes
            var query = _context.Clientes.AsQueryable();

            // Aplica filtros 
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(c => c.NombreCompleto.Contains(nombre));
            }

            if (!string.IsNullOrWhiteSpace(identificacion))
            {
                query = query.Where(c => c.Identificacion.Contains(identificacion));
            }

            if (!string.IsNullOrWhiteSpace(medidor))
            {
                query = query.Where(c => c.NumeroMedidor.Contains(medidor));
            }

            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
            {
                query = query.Where(c => c.Estado == estado);
            }


            return query.ToList();
        }
    }
}