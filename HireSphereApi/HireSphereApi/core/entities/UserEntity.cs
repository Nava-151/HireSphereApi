namespace HireSphereApi.core.entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class UserEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }
        //add here a mark
        //public double Mark{ get; set; } = 0;  

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required,JsonConverter(typeof(JsonStringEnumConverter)) ]
        public UserRole Role { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum UserRole
    {
        Candidate=0,
        Employer=1
    }

}
