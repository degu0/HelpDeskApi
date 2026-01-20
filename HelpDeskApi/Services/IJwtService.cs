using HelpDeskApi.Model;

namespace HelpDeskApi.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
