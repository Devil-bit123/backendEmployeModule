using backendEmployeModule.Models;
using backendEmployeModule.Services.Contract;
using Microsoft.EntityFrameworkCore;

namespace backendEmployeModule.Services.Implementation
{
    public class DepartmentService : IDepartmentService
    {
		private BackendEmployesModuleContext _backendEmployesModuleContext;

        public DepartmentService(BackendEmployesModuleContext backendEmployesModuleContext)
        {
            _backendEmployesModuleContext = backendEmployesModuleContext;
        }

        public async Task<List<Department>> GetDepartments()
        {
			try
			{
				List<Department> list = new List<Department>();
                list = await _backendEmployesModuleContext.Departments.ToListAsync();
                return list;

			}
			catch (Exception ex)
			{

				throw ex;
			}
        }
    }
}
