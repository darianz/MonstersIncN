using Login.Models;

namespace Login.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
        string CreateTokenN(User user);
        public bool IsTokenValid(string token);
        public string GetIdFromToken(string token);
    }
}
