namespace HireSphereApi.api
{
    using HireSphereApi.core.entities;
    using System.ComponentModel.DataAnnotations;

    public class UserPostModel
    {
        //public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }

        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }
    }

    

}
