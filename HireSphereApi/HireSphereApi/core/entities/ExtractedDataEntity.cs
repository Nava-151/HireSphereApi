
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

        
        public DateTime CreatedAt { get; set; } // תאריך העלאה
        public DateTime UpdatedAt { get; set; } // תאריך ע

        public string FileKey { get; set; } // מפתח הקובץ ב-S3

        public int IdResponse { get; set; }
        [ForeignKey(nameof(IdResponse))]
        public AIResponse Response { get; set; }

    }
}

