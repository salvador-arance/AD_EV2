using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrsEF.Controllers
{
    public class ClientesOcupacionesController : Controller
    {
        /* TODO: ClienteOcupaciónDto
			- DNI
			- Nombre
			- Teléfono
			- Número de personas
			- Nombre del albergue

			Flujo Controlador Recibe Datos JSON Devuelve RESPUESTA ->

			Recibe los datos del json

			VALIDACIONES CONTROLADOR
			Valida los datos del json
				dni correcto -> Regex DNI y no NULL
				nombre correcto -> No Null
				teléfono correcto -> Regex Teléfono y no NULL
				número personas correcto -> no puede ser menor que 1
				nombre del albergue correcto -> no NULL
			Guarda los datos del json en el DTO
			Llama al método del Repo y le pasa el DTO
			Devuelve lo que ha pasado en cada caso

			Flujo Repo Recibe ClienteOcupacionDTO Devuelve String ->

			VALIDACIONES REPOSITORIO
			albergue existe
				no existe: devuelve String error.
			albergue tiene plazas
				no tiene plazas suficientes: devuelve String error.

			si el dni no existe -> nuevo cliente
			si el dni existe -> se hace uptate del cliente

			si cliente no existe -> se devuelve mensaje indicando que la reserva se ha llevado a cabo y se ha creado el cliente
			si cliente existe -> se devuelve mensaje indicando que la reserva se ha llevado a cabo y el cliente ya existía*/
    }
}
