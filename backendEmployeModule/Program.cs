using backendEmployeModule.Models;
using backendEmployeModule.Services.Contract;
using backendEmployeModule.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using backendEmployeModule.Utilities;
using AutoMapper;
using backendEmployeModule.DTOS;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BackendEmployesModuleContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionServer"));
});

builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmployeService, EmployeService>();

// Aquí especificamos explícitamente el ensamblado que contiene los perfiles de AutoMapper.
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AppCORS_Policy", app =>
//    {
//        app.AllowAnyOrigin()
//        .AllowAnyMethod()
//        .AllowAnyHeader()
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCORS_Policy", app =>
    {
        app.WithOrigins("http://localhost:4200")
        .WithMethods("GET", "POST","PUT","DELETE")
        .WithHeaders("Content-Type", "Authorization");
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


#region APPI REQUEST

#region Departments
app.MapGet("/departments/list", async (
    IDepartmentService _departmentService,
    IMapper _mapper
    ) =>
{
    var departments = await _departmentService.GetDepartments();
    var departmentsDTO = _mapper.Map<List<Department>>(departments);

    if(departmentsDTO != null)
    {
        return Results.Ok(departmentsDTO);
    }
    else
    {
        return Results.NotFound();
    }

    
});


#endregion



#region Employes
app.MapGet("/employes/list", async (
    IEmployeService _employeService,
    IMapper _mapper
    ) =>
{
    var employes = await _employeService.GetAll();
    var employesDTO = _mapper.Map<List<Employe>>(employes);

    if (employesDTO != null)
    {
        return Results.Ok(employesDTO);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapPost("/employes/save", async (
    EmployeDTO dto,
    IEmployeService _employeService,
    IMapper _mapper
    ) =>
{

    var employe = _mapper.Map<Employe>(dto);
    employe.CreatedAt = DateTime.Now;
    var created = await _employeService.addEmploye(employe);

    if(created.Id != 0)
    {
        return Results.Ok(_mapper.Map<EmployeDTO>(created));
    }
    else
    {
        return Results.StatusCode(StatusCodes.Status400BadRequest);
    }

});

app.MapPut("/employes/update", async (
    int idEmploye,
    EmployeDTO dto,
    IEmployeService _employeService,
    IMapper _mapper
    ) =>
{
    var find = await _employeService.GetEmploye(idEmploye);
    if (find is null)
    {
        return Results.NotFound(find);
    }
    var employe = _mapper.Map<Employe>(dto);
    find.Name = employe.Name;
    find.LastName = employe.LastName;
    find.Email = employe.Email;
    find.Pay = employe.Pay;
    find.ContractDate = employe.ContractDate;

    var response = await _employeService.updateEmploye(find);

    if (response)
    {
        return Results.Ok(_mapper.Map<EmployeDTO>(find));
    }
    else
    {
        return Results.NotFound();
    }


});

app.MapDelete("/employes/delete/{idEmploye}", async (
    int idEmploye,
    IEmployeService _employeService,
    IMapper _mapper
    ) =>
{
    var find = await _employeService.GetEmploye(idEmploye);
    if (find is null)
    {
        return Results.NotFound(find);
    }

    var response = await _employeService.deleteEmploye(find);

    if (response)
    {
        return Results.Ok();
    }
    else
    {
        return Results.NotFound();
    }

});

#endregion


#endregion


app.UseCors("AppCORS_Policy");

app.Run();


