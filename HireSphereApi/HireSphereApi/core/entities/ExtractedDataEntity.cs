
namespace HireSphereApi.core.entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ExtractedData")]
    public class ExtractedDataEntity
    {
        [Key]
        public int Id { get; set; }

        public int CandidateId { get; set; }
        [ForeignKey(nameof(CandidateId))]
        public UserEntity? candidate { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
        public string FileKey { get; set; } 
        public decimal ? Mark { get; set; }
        public int IdResponse { get; set; }
        [ForeignKey(nameof(IdResponse))]
        public AIResponse? Response { get; set; }

    }
}

