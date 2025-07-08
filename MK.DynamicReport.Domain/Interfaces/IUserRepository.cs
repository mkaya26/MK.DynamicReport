using MK.DynamicReport.Domain.Entities;

namespace MK.DynamicReport.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
    }
}
