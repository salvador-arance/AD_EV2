using System.Text.Json.Serialization;

namespace GrsEF.Dtos
{
    public class ClienteOcupacionDTO
    {
        public string Dni { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Telefono { get; set; } = null!;

        [JsonPropertyName("cantidad_personas")]
        public int NumPersonas { get; set; }

        [JsonPropertyName("nombre_albergue")]
        public string NombreAlbergue { get; set; } = null!;
    }
}
