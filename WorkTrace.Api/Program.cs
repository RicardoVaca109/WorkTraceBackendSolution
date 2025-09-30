using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WorkTrace.Application;
using WorkTrace.Application.Configurations;
using WorkTrace.Data;
using WorkTrace.Data.Common.Setttings;
using WorkTrace.Logic;
using WorkTrace.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<WorkTraceDatabaseSettings>(
    builder.Configuration.GetSection("WorkTraceDatabase"));

builder.Services.AddDataServices();
builder.Services.AddRepositoriesServices();
builder.Services.AddLogicServices();
builder.Services.AddApplicationServices();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("ApplicationSettings"));


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(genConfig =>
    {
        genConfig.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "WorkTraceApi",
            Description = "Api Empresarial",
            Contact = new OpenApiContact
            {
                Name = "Ricardo",
                Email = "ricardo.vaca@udla.edu.ec",
            }
        });

        genConfig.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,     
            Scheme = "bearer",                  
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter:{your token}"
        });

        genConfig.AddSecurityRequirement(new OpenApiSecurityRequirement 
        {
            {
                new OpenApiSecurityScheme
                {
                     Reference = new OpenApiReference
                     {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                     }                   
                },
                new string []{}
            }        
        });
    });

var jwtConfiguration = builder.Configuration.GetSection("ApplicationSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey)),
        ValidIssuer = jwtConfiguration.Issuer,
        ValidAudience = jwtConfiguration.Audience,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
