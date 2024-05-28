using backendEmployeModule.Models;
using backendEmployeModule.Services.Contract;
using Microsoft.EntityFrameworkCore;

namespace backendEmployeModule.Services.Implementation
{
    public class EmployeService : IEmployeService
    {
        private BackendEmployesModuleContext _backendEmployesModuleContext;

        public EmployeService(BackendEmployesModuleContext backendEmployesModuleContext)
        {
            _backendEmployesModuleContext = backendEmployesModuleContext;   
        }

        public async Task<Employe> addEmploye(Employe employe)
        {
            try
            {
                _backendEmployesModuleContext.Employes.Add(employe);
               await _backendEmployesModuleContext.SaveChangesAsync();
                return employe;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> deleteEmploye(Employe employe)
        {
            try
            {
                _backendEmployesModuleContext.Employes.Remove(employe);
                await _backendEmployesModuleContext.SaveChangesAsync() ;
                return true;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<Employe>> GetAll()
        {
            try
            {
                List<Employe> employes = new List<Employe>();
                employes = await _backendEmployesModuleContext.Employes.Include(d=>d.IdDeptNavigation).ToListAsync();
                return employes;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Employe> GetEmploye(int idEmploye)
        {
            try
            {

                Employe? find = new Employe();
                find = await _backendEmployesModuleContext.Employes.Include(dpt =>dpt.IdDeptNavigation)
                    .Where(e=>e.Id == idEmploye).FirstOrDefaultAsync();
                return find;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> updateEmploye(Employe employe)
        {
            try
            {
                _backendEmployesModuleContext.Employes.Update(employe);
                await _backendEmployesModuleContext.SaveChangesAsync();
                return true;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
