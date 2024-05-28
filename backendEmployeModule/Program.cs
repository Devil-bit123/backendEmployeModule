using backendEmployeModule.Models;
using backendEmployeModule.Services.Contract;
using backendEmployeModule.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using backendEmployeModule.Utilities;
using AutoMapper;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


#region APPI REQUEST

#region Department
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




#endregion

app.Run();


