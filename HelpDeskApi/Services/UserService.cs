using HelpDeskApi.Data;
using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using HelpDeskApi.Repositories.Interfaces;
using HelpDeskApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace HelpDeskApi.Service;


public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public UserService(IUserRepository userRepository, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<User> CreatedUser(CreateUserDto dto)
    {
        var passwordHash = _passwordService.HashPassword(dto.Password);

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = passwordHash,
            DepartmentId = dto.DepartmentId
        };

        return await _userRepository.CreateAsync(user);
    }

    public async Task<List<ResponseUserDto>> GetAll()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<int> GetDepartmentByUser(int id)
    {
        return await _userRepository.GetDepartmentIdAsync(id);
    }

    public async Task<ResponseUserDto?> GetId(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
}
