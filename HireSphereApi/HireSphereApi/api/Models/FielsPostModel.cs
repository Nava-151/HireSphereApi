using HireSphereApi.core.entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireSphereApi.entities
{
    public class FilesPostModel
    {
        //public int Id { get; set; } 
        public string FileName { get; set; } 
        public string FileType { get; set; }
        public int OwnerId { get; set; } // בעל הקובץ

    }
}
