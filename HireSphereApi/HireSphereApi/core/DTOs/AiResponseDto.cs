using HireSphereApi.core.DTO;
using System.ComponentModel.DataAnnotations;

namespace HireSphereApi.core.DTOs
{
    public class AiResponseDto
    {
        public int? Experience { get; set; }
        public string? Education { get; set; }
        public string? Languages { get; set; }
        public string? EnglishLevel { get; set; }

        public decimal? Mark { get; set; }

    }
}
