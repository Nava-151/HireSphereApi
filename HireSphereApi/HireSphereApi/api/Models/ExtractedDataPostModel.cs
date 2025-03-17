using HireSphereApi.core.entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HireSphereApi.api.Models
{
    public class ExtractedDataPostModel
    {
        //אני לא בטוחה שצריך את המודל הזה
        public string Links { get; set; }
        public string? Technologies { get; set; }
        public decimal Experience { get; set; }
        public string Education { get; set; }
        public string PreviousWorkplaces { get; set; }
        public string ProgrammingLanguages { get; set; }



        public int CandidateId { get; set; }



        public string FileKey { get; set; } // מפתח הקובץ ב-S3

        public int IdResponse { get; set; }
    }
}
