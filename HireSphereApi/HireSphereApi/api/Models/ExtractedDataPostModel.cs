using HireSphereApi.core.entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HireSphereApi.api.Models
{
    public class ExtractedDataPostModel
    {

        public int CandidateId { get; set; }

        //public DateTime CreatedAt { get; set; } // תאריך העלאה
        //public DateTime UpdatedAt { get; set; } // תאריך ע

        public string FileKey { get; set; } // מפתח הקובץ ב-S3

        public int IdResponse { get; set; }


    }
}
