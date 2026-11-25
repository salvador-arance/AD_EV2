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

        public (string mensaje, Ocupacione? o) Add(ClienteOcupacionDTO clienteOcupacionDTO)
        {
            string mensaje = "";

            Albergue? albergue = _bdGrsContext.Albergues.SingleOrDefault(a => a.Nombre.ToLower() == clienteOcupacionDTO.NombreAlbergue.ToLower());

            if (albergue == null)
            {
                return ($"El albergue {clienteOcupacionDTO.NombreAlbergue} no existe.", null);
            }

            int plazasOcupadas = _bdGrsContext.Ocupaciones
                .Where(o => o.IdAlbergue == albergue.Id)
                .Sum(o => o.CantPersonas);

            int plazasLibres = albergue.TotalPlazas - plazasOcupadas;

            if (clienteOcupacionDTO.NumPersonas > plazasLibres) 
            {
                return ($"El albergue {clienteOcupacionDTO.NombreAlbergue} no tiene suficientes plazas.", null);
            }

            bool yaTieneReserva = _bdGrsContext.Ocupaciones
                 .Any(o => o.Dni == clienteOcupacionDTO.Dni && o.IdAlbergue == albergue.Id);

            if (yaTieneReserva)
            {
                return ($"El cliente '{clienteOcupacionDTO.Dni}' ya tiene registrada una ocupación en ESTE albergue: '{albergue.Nombre}'.", null);
            }

            var cliente = _bdGrsContext.Clientes.SingleOrDefault(c => c.Dni.ToLower() == clienteOcupacionDTO.Dni.ToLower());

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    Dni = clienteOcupacionDTO.Dni,
                    Nombre = clienteOcupacionDTO.Nombre,
                    Telefono = clienteOcupacionDTO.Telefono,
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
                    IdAlbergue = albergue.Id,
                    CantPersonas = clienteOcupacionDTO.NumPersonas
                };

                _bdGrsContext.Ocupaciones.Add(o);
                _bdGrsContext.SaveChanges();
                return (mensaje, o);
            }
            catch (Exception ex)
            {
                return ($"Error al guardar en la base de datos: {ex.Message}", null);
            }
        }
    }
}
