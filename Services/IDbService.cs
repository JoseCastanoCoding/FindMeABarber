namespace FindMeABarber.Services
{
    public interface IDbService
    {
        Task<T> GetAsync<T>(string command, object parameters);
        Task<List<T>> GetAll<T>(string command, object parameters);
        Task<int> EditData(string command, object parameters);
    }
}
