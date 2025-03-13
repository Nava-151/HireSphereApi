using HireSphereApi.core.DTO;

namespace HireSphereApi.core.DTOs
{
    public class FilteredCandidtes
    {
        public UserDto User { get; set; }
        public FileDto Resume { get; set; }
        public AiResponseDto Response { get; set; }
    }
}
