using HireSphereApi.api;
using HireSphereApi.core.DTO;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<UserDto?> GetUserById(int id);
    Task<UserDto?> GetUserByEmail(string email,string password);
    Task<UserDto> CreateUser(UserPostModel userModel);
    Task<bool> UpdateUser(int id, UserPostModel userModel);
    Task<bool> DeleteUser(int id);
}
