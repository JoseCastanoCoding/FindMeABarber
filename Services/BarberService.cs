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

        public async Task<bool> CreateBarber(Barber barber)
        {
            var result =
                await _dbService.EditData(
                    @"INSERT INTO public.barber (barberid, barbername, barbersurname, barberage, barberaddress, barbermobilenumber) 
                    VALUES (@BarberId, @BarberName, @BarberSurname, @BarberAge, @BarberAddress, @BarberMobileNumber)",
                    barber);

            return true;
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
