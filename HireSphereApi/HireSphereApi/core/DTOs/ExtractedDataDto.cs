using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HireSphereApi.core.DTO{
    using HireSphereApi.core.entities;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ExtractedDataDto
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }

        public string FileKey { get; set; } // מפתח הקובץ ב-S3

        public int IdResponse { get; set; }
    }
}

