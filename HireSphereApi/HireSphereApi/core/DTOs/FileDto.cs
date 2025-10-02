using System.ComponentModel.DataAnnotations.Schema;

namespace HireSphereApi.core.DTO
{
    public class FileDto
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public string S3Key { get; set; }
        public string FileContent { get; set; } 
        public int OwnerId { get; set; }

    }
}
