namespace Management.Interfaces
{
    public interface IDoorService
    {
        public Task<string> GetAvailableDoors(string authHeader);


    }
}
