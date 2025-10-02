using HireSphereApi.core.entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HireSphereApi.api.Models
{
    public class ExtractedDataPostModel
    {

<<<<<<< HEAD
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
        public string FileKey { get; set; } 
=======
        public int CandidateId { get; set; }

        public string FileKey { get; set; } 

>>>>>>> 5557d759eaca22b53a51c9fa8cfd4cf793350b5c
        public int IdResponse { get; set; }

    }
}
