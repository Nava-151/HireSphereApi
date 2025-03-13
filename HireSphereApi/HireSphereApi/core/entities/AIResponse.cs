using HireSphereApi.core.DTO;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireSphereApi.core.entities
{

    public class AIResponse
    {

        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }
        public int? Experience { get; set; }
        public string? WorkPlace { get; set; }
        public string? Languages { get; set; }
        public bool? RemoteWork { get; set; }
        public string? EnglishLevel { get; set; }
        public string? Links { get; set; }

    }
}
