using backendEmployeModule.Models;
namespace backendEmployeModule.Services.Contract
{
    public interface IEmployeService
    {
        Task<List<Employe>> GetAll();
        Task<Employe> GetEmploye(int idEmploye);
        Task<Employe> addEmploye(Employe employe);
        Task <bool> updateEmploye(Employe employe);
        Task <bool> deleteEmploye(Employe employe);
        Task<List<Employe>> GetEmployeDatesReport(DateTime gte, DateTime lte);

    }
}
