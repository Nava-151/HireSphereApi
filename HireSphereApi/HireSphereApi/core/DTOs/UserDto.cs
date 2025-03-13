namespace HireSphereApi.core.DTO
{
    using HireSphereApi.api;
    using HireSphereApi.core.entities;
    using System.ComponentModel.DataAnnotations;

    public class UserDto
    {
        public int Id { get; set; } 
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }
        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        //public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    

}
