using Management.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Management.Interfaces
{
    public interface IUserService
    {
        public Task<string> GetUserById(string authHeader);
        public Task UpdateUserById([FromBody] UpdateUserDto patchDoc, string authHeader);
    }
}
