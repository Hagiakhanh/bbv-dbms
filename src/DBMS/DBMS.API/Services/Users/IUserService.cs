using System.Collections.Generic;
using System.Threading.Tasks;
using DBMS.API.DTOs.Users;

namespace DBMS.API.Services.Users;

public interface IUserService
{
    Task<UserDto> CreateUserAsync(CreateUserRequest request);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<UserDto> UpdateUserAsync(int userId, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(int userId);
}
