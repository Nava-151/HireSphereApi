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
        public int? Experience { get; set; }
        public string? Education { get; set; }
        public string? Languages { get; set; }
        public string? EnglishLevel { get; set; }

    }
}
