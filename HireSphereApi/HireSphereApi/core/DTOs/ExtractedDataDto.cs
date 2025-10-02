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
<<<<<<< HEAD

        public string FileKey { get; set; } 

=======
        public UserEntity? Candidate { get; set; }
        public string FileKey { get; set; }
>>>>>>> 5557d759eaca22b53a51c9fa8cfd4cf793350b5c
        public int IdResponse { get; set; }
        public AIResponse? Response { get; set; }

    }
}

