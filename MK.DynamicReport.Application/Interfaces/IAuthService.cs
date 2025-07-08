using MK.DynamicReport.Domain.DTOs;

namespace MK.DynamicReport.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(LoginRequestDto request);
    }
}
