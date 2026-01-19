using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using HelpDeskApi.Models;

namespace HelpDeskApi.Mappers
{
    public class UserMapper
    {
        public static ResponseUserDto ToResponseDto(User user)
        {
            return new ResponseUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Department = user.Department.Name,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
