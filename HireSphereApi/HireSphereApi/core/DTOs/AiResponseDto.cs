using HireSphereApi.core.DTO;

namespace HireSphereApi.core.DTOs
{
    public class AiResponseDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Experience { get; set; }
        public string? WorkPlace { get; set; }
        public string? Languages { get; set; }
        public bool? RemoteWork { get; set; }
        public string? EnglishLevel { get; set; }
    }
}
