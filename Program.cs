using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Visitor_Check_Application.Interfaces;
using Visitor_Check_Application.Models;
using Visitor_Check_Application.Respositiries;
using Visitor_Check_Application.Service_Layer;
using Visitor_Check_Application.Services;
using Visitor_Check_Application.MiddleWare;
using Visitor_Check_Application.Src.Application.Interfaces;
using Visitor_Check_Application.Infrastructure.Respositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<VisitorCheckDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDivisionInterface,DivisionRepository>();
builder.Services.AddScoped<DivisionService>();
builder.Services.AddScoped<HospitalService>();
builder.Services.AddScoped<IHospitalInterface, HospitalRepository>();
builder.Services.AddScoped<IPatientInterface, PatientRepository>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection(); //Redirecting to the secure conenction
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
