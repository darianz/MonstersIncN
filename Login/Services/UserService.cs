using Login.Controllers;
using Login.DTOs;
using Login.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Login.Services
{
    public class UserService
    {

        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        public UserService(AppDbContext context, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<UserService>();
            _context = context;
        }

        public async Task<User> RegisterUser(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) throw new Exception("Username is taken");
            // Validate password requirements
            //if (registerDto.Password.Length < 8)
            //{
            //    throw new Exception("Password must be at least 8 characters long");
            //}
            //if (!registerDto.Password.Any(char.IsUpper))
            //{
            //     throw new Exception("Password must have at least one capital letter");
            //}
            //if (!registerDto.Password.Any(char.IsDigit))
            //{
            //    throw new Exception("Password must have at least one digit");
            //}
            //if (!registerDto.Password.Any(char.IsSymbol))
            //{
            //    throw new Exception("Password must have at least one non-alphanumeric character");
            //}

            using var hmac = new HMACSHA512();
            var user = new User
            {
                Username = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
                TentaclesNumber = registerDto.TentaclesNumber,
                PhoneNumber = registerDto.PhoneNumber,
                LastName = registerDto.LastName,
                FirstName = registerDto.FirstName
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == loginDto.Username);
            if (user == null) {
                _logger.LogError("Username is invalid");
                throw new Exception("Username is invalid"); 
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) throw new Exception("Invalid password");
            }
            return user;
        }

        public async Task UpdateUser(int id, JsonPatchDocument<UpdateUserDto> patchDoc)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var updateUserDto = new UpdateUserDto
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                TentaclesNumber = user.TentaclesNumber
            };

           

            user.Username = updateUserDto.Username;
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.TentaclesNumber = updateUserDto.TentaclesNumber;
            await _context.SaveChangesAsync();

        }

        public async Task<User> GetUserById(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving user");
                throw new Exception("Error retrieving user", ex);
            }
        }

        public async Task<bool> UserExists(string username)
        {
            try
            {
                return await _context.Users.AnyAsync(x => x.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error checking if user exists");
                throw new Exception("Error checking if user exists", ex);
            }
        }


    }
}
