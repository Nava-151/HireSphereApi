
using HireSphereApi.Data;
using HireSphereApi.entities;
using HireSphereApi.Service.Iservice;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphereApi.api;
using HireSphereApi.core.DTO;
using HireSphereApi.core.entities;
using System.Text.Json;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto> CreateUser(UserPostModel userModel)
    {
        var userEntity = _mapper.Map<UserEntity>(userModel);
        _context.Users.Add(userEntity);
        Console.WriteLine($"Received JSON: {JsonSerializer.Serialize(userModel)}");
        await _context.SaveChangesAsync();
        return _mapper.Map<UserDto>(userEntity);
    }

    public async Task<bool> UpdateUser(int id, UserPostModel userModel)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null) return false;

        _mapper.Map(userModel, existingUser);
        existingUser.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUser(int id)
    {
        
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserDto?> GetUserByEmail(LoginUser loginU)
    {
        var user=await _context.Users.FirstOrDefaultAsync(u=>u.Email==loginU.Email);
        if (user == null|| !BCrypt.Net.BCrypt.Verify(loginU.PasswordHash, user.PasswordHash)) return null;
       return _mapper.Map<UserDto>(user);
    }
}
