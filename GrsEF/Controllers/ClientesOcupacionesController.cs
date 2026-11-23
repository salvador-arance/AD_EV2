using _4VGymAPI.Models;
using GrsEF.Dtos;
using GrsEF.Models;
using GrsEF.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace GrsEF.Controllers
{
    [ApiController]
    [Route("api/Ocupaciones")]
    public class ClientesOcupacionesController : ControllerBase
    {
        private readonly ClientesOcupacionesRepository _clientesOcupacionesRepository;
        public ClientesOcupacionesController(ClientesOcupacionesRepository clientesOcupacionesRepository)
        {
            _clientesOcupacionesRepository = clientesOcupacionesRepository;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Ocupacione), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RespuestaError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RespuestaError), StatusCodes.Status409Conflict)]

        public ActionResult<Ocupacione> CrearOcupacion([FromBody] ClienteOcupacionDTO coDTO)
        {
            string patronDni = @"^\d{8}[A-HJ-NP-TV-Z]$";
            string patronTelefono = @"^[6-9]\d{8}$";

            if (String.IsNullOrWhiteSpace(coDTO.Dni) || !Regex.IsMatch(coDTO.Dni.ToUpper(), patronDni))
            {
                return BadRequest(new RespuestaError(400, "El dni no es válido"));
            }

            if (String.IsNullOrWhiteSpace(coDTO.Nombre))
            {
                return BadRequest(new RespuestaError(400, "El nombre no puede estar vacío"));
            }

            if (String.IsNullOrWhiteSpace(coDTO.Telefono) || !Regex.IsMatch(coDTO.Telefono, patronTelefono))
            {
                return BadRequest(new RespuestaError(400, "El teléfono no es válido"));
            }

            if (coDTO.NumPersonas < 1)
            {
                return BadRequest(new RespuestaError(400, "Error en el número de personas: no puede ser inferior a 1."));
            }

            if (String.IsNullOrWhiteSpace(coDTO.NombreAlbergue))
            {
                return BadRequest(new RespuestaError(400, "El nombre del albergue no puede estar vacío."));
            }

            (string mensaje, Ocupacione? o) = _clientesOcupacionesRepository.Add(coDTO);

            if (o == null)
            {
                if (mensaje.StartsWith("El"))
                {
                    return Conflict(new RespuestaError(409, mensaje));
                }
                return BadRequest(new RespuestaError(400, mensaje));
            }
            return StatusCode(201, new
            {
                ocupacion = coDTO,
                información = mensaje,
                clienteDto = new { dni = coDTO.Dni, nombre = coDTO.Nombre, telefono = coDTO.Telefono }
            });
        }
    }
}
