

using CwkSocial.Api.Filters;
using CwkSocial.Application.UserProfiles.Queries;
using CwkSocial.Dal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(config =>
{
    config.Filters.Add(typeof(CwkSocialExceptionHandler));
    //config.Filters.Add(typeof(ValidateModelAttribute));
});

// builder.RegisterServices(typeof(Program));
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Program).Assembly);
 });
builder.Services.AddMediatR(cfg =>
{
    //cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    //cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(nameof(GetAllUserProfiles)));
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly, typeof(GetAllUserProfiles).Assembly);
});
                
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//  app.RegisterPipelineComponents(typeof(Program));
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
