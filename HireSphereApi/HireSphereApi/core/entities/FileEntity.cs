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
        public long Size { get; set; } = 0;// גודל הקובץ בבתים
        [Required]
        //public string S3Key { get; set; } = "Undefined";// מזהה הקובץ ב-S3 (לדוגמה: 'uploads/user1/file.jpg')
        public int OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public UserEntity owner { get; set; }

        public DateTime CreatedAt { get; set; } // תאריך העלאה
        public DateTime UpdatedAt { get; set; } // תאריך עדכון אחרון

        public bool IsDeleted { get; set; } // דגל למחיקה רכה
        public int an { get; set; }
    }
}
