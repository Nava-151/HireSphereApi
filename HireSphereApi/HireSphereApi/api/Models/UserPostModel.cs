namespace HireSphereApi.api
{
    using HireSphereApi.core.entities;
    using System.ComponentModel.DataAnnotations;

    public class UserPostModel
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }

        public string? Phone { get; set; }
        public string? S3Key { get; set; }


        public string? PasswordHash { get; set; }

        public UserRole? Role { get; set; }
    }

    

}
