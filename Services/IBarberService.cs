using FindMeABarber.Models;

namespace FindMeABarber.Services
{
    public interface IBarberService
    {
        Task<Barber> CreateBarber(Barber barber);
        Task<Barber> GetBarber(int barberId);
        Task<List<Barber>> GetBarberList();
        Task<Barber> UpdateBarber (Barber barber);
        Task<bool> DeleteBarber(int barberId);
    }
}
