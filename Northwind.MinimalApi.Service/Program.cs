using Microsoft.AspNetCore.Mvc;
using Northwind.EntityModels;
using System.Security.Cryptography.Xml;
using System.Text.Json.Serialization;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddNorthwindContext();
builder.Services.Configure<HttpJsonOptions>(option =>
{
    option.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.MapGet("api/employees", (
 [FromServices] NorthwindContext db) =>
 Results.Json(db.Employees))
 .WithName("GetEmployees")
 .Produces<Employee[]>(StatusCodes.Status200OK);

app.MapGet("api/employees/{id:int}", (
 [FromServices] NorthwindContext db,
 [FromRoute] int id) =>
{
    Employee? employee = db.Employees.Find(id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Json(employee);
    }
})
 .WithName("GetEmployeesById")
 .Produces<Employee>(StatusCodes.Status200OK)
 .Produces(StatusCodes.Status404NotFound);


app.MapGet("api/employees/{country}", (
 [FromServices] NorthwindContext db,
 [FromRoute] string country) =>
 Results.Json(db.Employees.Where(employee =>
 employee.Country == country)))
 .WithName("GetEmployeesByCountry")
 .Produces<Employee[]>(StatusCodes.Status200OK);

app.MapPost("api/employees", async ([FromBody] Employee employee,
 [FromServices] NorthwindContext db) =>
{
    db.Employees.Add(employee);
    await db.SaveChangesAsync();
    return Results.Created($"api/employees/{employee.EmployeeId}", employee);
 })
 .Produces<Employee>(StatusCodes.Status201Created);


app.Run();
