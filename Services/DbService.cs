using Dapper;
using Npgsql;
using System.Data;

namespace FindMeABarber.Services
{
    public class DbService : IDbService
    {
        private readonly IDbConnection _dbConnection;
        public DbService(IConfiguration configuration)
        {
            _dbConnection = new NpgsqlConnection(configuration.GetConnectionString("FindMeABarberTest"));
        }

        public async Task<T> GetAsync<T>(string command, object parameters)
        {
            T result;
            result = (await _dbConnection.QueryAsync<T>(command, parameters).ConfigureAwait(false)).FirstOrDefault();
            return result;
        }

        public async Task<List<T>> GetAll<T>(string command, object parms)
        {

            List<T> result = new List<T>();

            result = (await _dbConnection.QueryAsync<T>(command, parms)).ToList();

            return result;
        }

        public async Task<int> EditData(string command, object parms)
        {
            int result;

            result = await _dbConnection.ExecuteAsync(command, parms);

            return result;
        }
    }
}
