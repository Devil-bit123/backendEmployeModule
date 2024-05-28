using backendEmployeModule.Models;

namespace backendEmployeModule.Services.Contract


{
    public interface IDepartmentService
    {
        Task<List<Department>> GetDepartments();
    }
}
