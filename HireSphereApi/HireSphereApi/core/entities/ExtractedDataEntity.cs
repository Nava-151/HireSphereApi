using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HireSphereApi.core.entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ExtractedDataEntity
    {
        [Key]
        public int Id { get; set; }
        public int CandidateId { get; set; }
        [ForeignKey(nameof(CandidateId))]
        public UserEntity candidate { get; set; }
        public string Links { get; set; }
        public string? Technologies { get; set; }
        public decimal Experience { get; set; }
        public string Education { get; set; }
        public string PreviousWorkplaces { get; set; }
        public string ProgrammingLanguages { get; set; }
        public DateTime CreatedAt { get; set; } // תאריך העלאה
        public DateTime UpdatedAt { get; set; } // תאריך ע

    }
}

