using backendEmployeModule.Models;
using backendEmployeModule.Services.Contract;
using backendEmployeModule.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using backendEmployeModule.Utilities;
using AutoMapper;
using backendEmployeModule.DTOS;
using OfficeOpenXml;
using System.Drawing;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;



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
        .WithMethods("GET", "POST", "PUT", "DELETE")
        .WithHeaders("Content-Type", "Authorization");
    });
});


ExcelPackage.LicenseContext = LicenseContext.NonCommercial;





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
    var departmentsDTO = _mapper.Map<List<DepartmentDTO>>(departments);

    if (departmentsDTO != null)
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
    var employesDTO = _mapper.Map<List<EmployeDTO>>(employes);

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

    if (created.Id != 0)
    {
        return Results.Ok(_mapper.Map<EmployeDTO>(created));
    }
    else
    {
        return Results.StatusCode(StatusCodes.Status400BadRequest);
    }

});

app.MapPut("/employes/update/{idEmploye}", async (
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
    find.IdDept = employe.IdDept;

    var response = await _employeService.updateEmploye(employe);

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

app.MapPost("/employes/report", async (
    HttpContext context,
    IEmployeService _employeService,
    IMapper _mapper
    ) =>
{
    // Leer el cuerpo de la solicitud
    string requestBody;
    using (var reader = new StreamReader(context.Request.Body))
    {
        requestBody = await reader.ReadToEndAsync();
    }

    // Parsear el cuerpo de la solicitud como JSON
    dynamic requestData = JsonConvert.DeserializeObject(requestBody);

    // Obtener los valores de gte y lte del cuerpo de la solicitud
    DateTime gte = DateTime.Parse(requestData.gte.ToString());
    DateTime lte = DateTime.Parse(requestData.lte.ToString());

    // Utilizar los valores gte y lte en tu lógica
    var employes = await _employeService.GetEmployeDatesReport(gte, lte);
    var employesDTO = _mapper.Map<List<EmployeDTO>>(employes);



    // Configuración de EPPlus para crear el archivo Excel
    using (var package = new ExcelPackage())
    {
        var worksheet = package.Workbook.Worksheets.Add("Empleados");

        // Configurar encabezados
        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "Name";
        worksheet.Cells[1, 3].Value = "Last name";
        worksheet.Cells[1, 4].Value = "Email";
        worksheet.Cells[1, 5].Value = "Pay";
        worksheet.Cells[1, 6].Value = "Contract Date";
        worksheet.Cells[1, 7].Value = "Department";

        // Aplicar estilos a los encabezados
        using (var range = worksheet.Cells[1, 1, 1, 7])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        }

        // Llenar los datos de empleados
        for (int i = 0; i < employesDTO.Count; i++)
        {
            var employe = employesDTO[i];
            worksheet.Cells[i + 2, 1].Value = employe.Id;
            worksheet.Cells[i + 2, 2].Value = employe.Name;
            worksheet.Cells[i + 2, 3].Value = employe.LastName;
            worksheet.Cells[i + 2, 4].Value = employe.Email;
            worksheet.Cells[i + 2, 5].Value = employe.Pay;
            worksheet.Cells[i + 2, 6].Value = employe.ContractDate;
            worksheet.Cells[i + 2, 7].Value = employe.NameDept;
        }

        // Ajustar el ancho de las columnas
        worksheet.Cells.AutoFitColumns();

        // Guardar el archivo en un MemoryStream
        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;

        // Devolver el archivo como una respuesta
        return Results.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Empleados.xlsx");
    }

});



app.MapPost("/employes/graphic", async (
    HttpContext context,
    IEmployeService _employeService,
    IMapper _mapper
    ) =>
{
    // Leer el cuerpo de la solicitud
    string requestBody;
    using (var reader = new StreamReader(context.Request.Body))
    {
        requestBody = await reader.ReadToEndAsync();
    }

    try
    {
        // Parsear el cuerpo de la solicitud como JSON
        var requestData = JsonConvert.DeserializeObject<filter>(requestBody);

        // Validar los datos del cuerpo de la solicitud
        if (requestData == null || string.IsNullOrEmpty(requestData.gte) || string.IsNullOrEmpty(requestData.lte))
        {
            return Results.BadRequest("Los parámetros gte y lte son requeridos.");
        }


        if(requestData != null && requestData.is_download == true)
        {

            var employes = await _employeService.GetEmployeGraphicssReport(requestData);
            var employesDTO = _mapper.Map<List<EmployeDTO>>(employes);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Empleados");

                // Configurar encabezados
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Last name";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Pay";
                worksheet.Cells[1, 6].Value = "Contract Date";
                worksheet.Cells[1, 7].Value = "Department";

                // Aplicar estilos a los encabezados
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // Llenar los datos de empleados
                for (int i = 0; i < employesDTO.Count; i++)
                {
                    var employe = employesDTO[i];
                    worksheet.Cells[i + 2, 1].Value = employe.Id;
                    worksheet.Cells[i + 2, 2].Value = employe.Name;
                    worksheet.Cells[i + 2, 3].Value = employe.LastName;
                    worksheet.Cells[i + 2, 4].Value = employe.Email;
                    worksheet.Cells[i + 2, 5].Value = employe.Pay;
                    worksheet.Cells[i + 2, 6].Value = employe.ContractDate;
                    worksheet.Cells[i + 2, 7].Value = employe.NameDept;
                }

                worksheet.Cells.AutoFitColumns();

                // Guardar el archivo en un MemoryStream
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string fileName = "Employes_resport_from_"+requestData.gte+"_to_"+ requestData.lte+"_.xlsx";

                // Devolver el archivo como una respuesta
                return Results.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);


            }

        }
        else
        {
            // Lógica de negocio aquí, por ejemplo, obtener datos del servicio de empleados
            var employes = await _employeService.GetEmployeGraphicssReport(requestData);
            var employesDTO = _mapper.Map<List<EmployeDTO>>(employes);

            // Devolver los datos procesados
            return Results.Ok(employesDTO);
        }
        



       
    }
    catch (JsonException ex)
    {
        return Results.BadRequest($"Error al parsear el JSON: {ex.Message}");
    }


    catch (FormatException ex)
    {
        return Results.BadRequest($"Error en el formato de fechas: {ex.Message}");
    }
});




#endregion


#endregion


app.UseCors("AppCORS_Policy");

app.Run();


