using HireSphereApi.core.entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireSphereApi.entities
{
    public class FilesPostModel
    {
        public string FileName { get; set; } 
        public string FileType { get; set; }
        public int OwnerId { get; set; } 
        public long Size { get; set; } = 0;


    }
}
