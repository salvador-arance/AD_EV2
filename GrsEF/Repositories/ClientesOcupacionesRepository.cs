using GrsEF.Data;
using GrsEF.Dtos;
using GrsEF.Models;
using Microsoft.AspNetCore.Razor.Hosting;

namespace GrsEF.Repositories
{
    public class ClientesOcupacionesRepository
    {
        private readonly BdGrsContext _bdGrsContext;
        public ClientesOcupacionesRepository(BdGrsContext bdGrsContext) 
        {
            _bdGrsContext = bdGrsContext;
        }

        public (bool exito, string mensaje, Ocupacione? o) Add(ClienteOcupacionDTO clienteOcupacionDTO)
        {
            string mensaje = "";
            string nombreAlbergue = clienteOcupacionDTO.NombreAlbergue;
            bool existe = _bdGrsContext.Albergues.Any(a => a.Nombre.ToLower() == nombreAlbergue.ToLower());
            if (!existe)
            {
                return (false, $"El albergue {nombreAlbergue} no existe.", null);
            }

            Albergue albergue = _bdGrsContext.Albergues.SingleOrDefault(a => a.Nombre.ToLower() == nombreAlbergue.ToLower())!;

            string idAlbergue = albergue.Id!;

            int plazasOcupadas = _bdGrsContext.Ocupaciones
                .Where(o => o.IdAlbergue == idAlbergue)
                .Sum(o => o.CantPersonas);

            int plazasLibres = albergue.TotalPlazas - plazasOcupadas;

            if (clienteOcupacionDTO.NumPersonas > plazasLibres) 
            {
                return (false, $"El albergue {nombreAlbergue} no tiene suficientes plazas.", null);
            }

            bool yaTieneReserva = _bdGrsContext.Ocupaciones
                 .Any(o => o.Dni == clienteOcupacionDTO.Dni && o.IdAlbergue == idAlbergue);

            if (yaTieneReserva)
            {
                return (false, $"El cliente '{clienteOcupacionDTO.Dni}' ya tiene registrada una ocupación en ESTE albergue: '{albergue.Nombre}'.", null);
            }

            var cliente = _bdGrsContext.Clientes.SingleOrDefault(c => c.Dni.ToLower() == clienteOcupacionDTO.Dni.ToLower());

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    Dni = clienteOcupacionDTO.Dni,
                    Nombre = clienteOcupacionDTO.Nombre,
                    Telefono = clienteOcupacionDTO.Telefono,
                    Ocupaciones = []
                };

                _bdGrsContext.Clientes.Add(cliente);
                mensaje = "Ocupación realizada con éxito. El cliente no existía y ha sido creado";
            }
            else if (cliente.Nombre != clienteOcupacionDTO.Nombre || cliente.Telefono != clienteOcupacionDTO.Telefono)
            {
                cliente.Nombre = clienteOcupacionDTO.Nombre;
                cliente.Telefono = clienteOcupacionDTO.Telefono;
                mensaje = "Se han actualizado los datos del cliente y se ha añadido la ocupación al albergue.";
            }
            else
            {
                mensaje = "El cliente ya existía con esos datos y se ha añadido la ocupación al albergue.";
            }

            try
            {
                Ocupacione o = new Ocupacione
                {
                    Dni = clienteOcupacionDTO.Dni,
                    IdAlbergue = idAlbergue,
                    CantPersonas = clienteOcupacionDTO.NumPersonas
                };

                _bdGrsContext.Ocupaciones.Add(o);

                cliente!.Ocupaciones.Add(o);
                _bdGrsContext.SaveChanges();
                return (true, mensaje, o);
            }
            catch (Exception ex)
            {
                return (false, $"Error al guardar en la base de datos: {ex.Message}", null);
            }
        }
    }
}
