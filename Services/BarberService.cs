using FindMeABarber.Models;

namespace FindMeABarber.Services
{
    public class BarberService : IBarberService
    {
        private readonly IDbService _dbService;

        public BarberService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Barber> CreateBarber(Barber barber)
        {
            var sql = @"INSERT INTO public.barber (barbername, barbersurname, barberage, barberaddress, barbermobilenumber)
                        VALUES (@BarberName, @BarberSurname, @BarberAge, @BarberAddress, @BarberMobileNumber)
                        RETURNING barberid;";

            // Use _dbService.EditData to insert and get the new id, or add a method to IDbService/DbService to support QuerySingleAsync
            var newId = await _dbService.GetAsync<int>(sql, barber);
            barber.BarberId = newId;
            return barber;
        }

        
        public async Task<List<Barber>> GetBarberList()
        {
            var barberList = await _dbService.GetAll<Barber>(@"SELECT * FROM public.barber", new { });
            return barberList;
        }

        public async Task<Barber> GetBarber(int barberId)
        {
            var barber = await _dbService.GetAsync<Barber>(@"SELECT * FROM public.barber WHERE barberId = @barberId", new { barberId });
            return barber;
        }

        public async Task<Barber> UpdateBarber(Barber barber)
        {
            var updateBarber =
                await _dbService.EditData(
                    @"UPDATE public.barber 
                        SET barberName = @BarberName, barberSurname = @BarberSurname, barberAge = @BarberAge, barberAddress = @BarberAddress, barberMobileNumber = @BarberMobileNumber
                        WHERE barberId = @BarberId",
                    barber);
            return barber;
        }

        public async Task<bool> DeleteBarber(int barberId)
        {
            var deleteBarber = await _dbService.EditData(@"DELETE FROM public.barber WHERE barberId = @BarberId", new { barberId });
            return true;
        }
    }
}
