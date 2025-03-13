using HireSphereApi.core.entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireSphereApi.entities
{
    public class FileEntity
    {
        public int Id { get; set; } 
        public string FileName { get; set; } 
        public string FileType { get; set; }
        [Required]
        public long Size { get; set; } = 0;// גודל הקובץ בבתים
        [Required]
        public string S3Key { get; set; } = "Undefined";// מזהה הקובץ ב-S3 (לדוגמה: 'uploads/user1/file.jpg')


        public int OwnerId { get; set; } // בעל הקובץ
        [ForeignKey(nameof(OwnerId))]
        public UserEntity owner { get; set; } // קשר עם המשתמש (הנחת שזו מחלקה קיימת)

        public DateTime CreatedAt { get; set; } // תאריך העלאה
        public DateTime UpdatedAt { get; set; } // תאריך עדכון אחרון

        public bool IsDeleted { get; set; } // דגל למחיקה רכה
    }
}
