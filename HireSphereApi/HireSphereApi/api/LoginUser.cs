using HireSphereApi.core.entities;

namespace HireSphereApi.api
{
    public class LoginUser
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
    }
}
