using HireSphereApi.core.DTO;
using System.ComponentModel.DataAnnotations;

namespace HireSphereApi.core.DTOs
{
    public class AiResponseDto
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int? Experience { get; set; }
        public string? WorkPlace { get; set; }
        public string? Languages { get; set; }
        public bool? RemoteWork { get; set; }
        public string? EnglishLevel { get; set; }
        public string? Links { get; set; }

    }
}
