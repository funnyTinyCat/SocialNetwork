

using CwkSocial.Api.Filters;
using CwkSocial.Application.UserProfiles.Queries;
using CwkSocial.Dal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using CwkSocial.Application.Services;

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

builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;

})
    .AddEntityFrameworkStores<DataContext>();

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
//builder.Services.AddIdentity<IdentityOptions, IdentityRole>(options =>
//{
//    options.Password.RequireDigit = false;
//    options.Password.RequiredLength = 5;
//    options.Password.RequireUppercase = false;
//    options.Password.RequireLowercase = false;
//    options.Password.RequireNonAlphanumeric = false;

//});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true; 
        options.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SigningKey"]!)),
            ValidateIssuer = true,
            // if up is true => ValidIssuer = jwtSettings.Issuer,
            ValidIssuer = "CwkSocial",
            ValidateAudience = true,
            // if up is true => ValidAudiences = jwtSettings.Audiences
            ValidAudience = "https://localhost:7280",
            RequireExpirationTime = false,
            ValidateLifetime = true
        };
        options.ClaimsIssuer = "CwkSocial";
    });

builder.Services.AddScoped<IdentityService>();
                
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//  app.RegisterPipelineComponents(typeof(Program));
app.UseHttpsRedirection();

app.UseAuthentication();    
app.UseAuthorization();

app.MapControllers();

app.Run();
