using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HireSphereApi.core.entities
{
    public class FileEntity
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        [Required]
        public long Size { get; set; } = 0;
        [Required]
        public int OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public UserEntity owner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } 
        public int an { get; set; }
    }
}
