using AutoMapper;
using backendEmployeModule.DTOS;
using backendEmployeModule.Models;
using System.Globalization;

namespace backendEmployeModule.Utilities
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            #region Departments
            CreateMap<Department, DepartmentDTO>().ReverseMap();
            #endregion

            #region Employes

            /*Method to map Employes from Employe (Model)
             * To EmployeDTO
             * 
             */

            CreateMap<Employe, EmployeDTO>()
                .ForMember(destiny => destiny.NameDept,
                opt => opt.MapFrom(origin => origin.IdDeptNavigation.Name)
                )
                .ForMember(destiny => destiny.ContractDate,
                opt => opt.MapFrom(origin => origin.ContractDate.Value.ToString("yyyy-MM-dd"))
                );

            CreateMap<EmployeDTO, Employe>()
                .ForMember(destiny => destiny.IdDeptNavigation,
                opt => opt.Ignore()
                )
                .ForMember(destiny => destiny.ContractDate,
                opt => opt.MapFrom(origin => DateTime.ParseExact(origin.ContractDate,"dd/MM/yyyy", CultureInfo.InvariantCulture))
                );
            #endregion


        }
    }
}
