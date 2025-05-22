using HireSphereApi.core.entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HireSphereApi.api.Models
{
    public class ExtractedDataPostModel
    {

        public int CandidateId { get; set; }

        public string FileKey { get; set; } 

        public int IdResponse { get; set; }


    }
}
