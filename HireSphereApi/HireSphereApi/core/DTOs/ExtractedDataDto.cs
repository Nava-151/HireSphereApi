using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HireSphereApi.core.DTO{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ExtractedDataDto
    {
        public int Id { get; set; }
        public string Links { get; set; }
        public string? Technologies { get; set; } 
        public decimal Experience { get; set; }
        public string Education { get; set; }
        public string PreviousWorkplaces { get; set; }
        public string ProgrammingLanguages { get; set; }

    }
}

