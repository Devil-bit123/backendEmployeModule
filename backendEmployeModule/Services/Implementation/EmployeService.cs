using backendEmployeModule.Models;
using backendEmployeModule.Services.Contract;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

using System.IO;
using System.Drawing;

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


        public async Task<List<Employe>> GetEmployeDatesReport(DateTime gte, DateTime lte)
        {
            try
            {
                List<Employe> employes = new List<Employe>();
                employes = await _backendEmployesModuleContext.Employes.Include(d => d.IdDeptNavigation).Where(e=>e.ContractDate >= gte && e.ContractDate<=lte) .ToListAsync();
                return employes;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<Employe>> GetEmployeGraphicssReport(filter _filter)
        {
            try
            {
                // Obtener todos los empleados y sus respectivas navegaciones

                var employes = await _backendEmployesModuleContext.Employes.Include(d => d.IdDeptNavigation).ToListAsync();
                // Filtrar empleados por fechas gte y lte
                if (!string.IsNullOrEmpty(_filter.gte) && !string.IsNullOrEmpty(_filter.lte))
                {
                    DateTime gteDate = DateTime.Parse(_filter.gte);
                    DateTime lteDate = DateTime.Parse(_filter.lte);



                    employes = employes.Where(e => e.ContractDate >= gteDate && e.ContractDate <= lteDate).ToList();
                }

                // Filtrar por salario mínimo y máximo si están presentes
                if (_filter.min_pay.HasValue)
                {
                    employes = employes.Where(e => e.Pay >= _filter.min_pay.Value).ToList();
                }

                if (_filter.max_pay.HasValue)
                {
                    employes = employes.Where(e => e.Pay <= _filter.max_pay.Value).ToList();
                }
                                
                employes = employes.OrderByDescending(e => e.Pay).ToList();            
                              

                return employes;

            }
            catch (Exception e)
            {
                // Manejo de excepciones más específico puede ser añadido aquí
                throw new Exception("Error al obtener el reporte gráfico de empleados", e);
            }
        }


    }
}
