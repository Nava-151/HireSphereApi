using System.ComponentModel.DataAnnotations.Schema;

namespace HireSphereApi.core.DTO
{
    public class FileDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } 
        public string FileType { get; set; }
        public long Size { get; set; } // גודל הקובץ בבתים

        public DateTime CreatedAt { get; set; } // תאריך העלאה
        public DateTime UpdatedAt { get; set; } // תאריך עדכון אחרון

    }
}
