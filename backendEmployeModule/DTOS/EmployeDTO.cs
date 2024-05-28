using backendEmployeModule.Models;

namespace backendEmployeModule.DTOS
{
    public class EmployeDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public decimal? Pay { get; set; }

        public string? ContractDate { get; set; }
               
        public int? IdDept { get; set; }
        public string? NameDept { get; set; }


    }
}
