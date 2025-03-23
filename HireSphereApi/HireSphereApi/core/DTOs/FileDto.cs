using System.ComponentModel.DataAnnotations.Schema;

namespace HireSphereApi.core.DTO
{
    public class FileDto
    {
        //public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public string S3Key { get; set; }
        public string FileContent { get; set; } // טקסט מלא של הקובץ
        public int OwnerId { get; set; }// תאריך עדכון אחרון

    }
}
