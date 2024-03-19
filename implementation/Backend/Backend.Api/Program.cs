using System.Text;
using Backend.Api.Models;
using Backend.Api.Options;
using Backend.Api.Repositories;
using Backend.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(
  options => options.UseSqlite("Data Source=./database.db")
);

builder.Services.AddTransient<IAssociateRepository, AssociateRepository>();
builder.Services.AddTransient<IRepository<Schedule>, Repository<Schedule>>();
builder.Services.AddTransient<IRepository<WorkEvent>, Repository<WorkEvent>>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IVacationRepository, VacationRepository>();

builder.Services.AddTransient<IAssociateService, AssociateService>();
builder.Services.AddTransient<IScheduleService, ScheduleService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IVacationService, VacationService>();

var jwtSettings = new JwtSettings();

builder.Configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddCors(
  opt =>
    opt.AddDefaultPolicy(
      policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    )
);

builder.Services
  .AddControllers()
  .AddNewtonsoftJson(options =>
  {
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft
      .Json
      .DateTimeZoneHandling
      .Utc;
  });

TokenValidationParameters tokenValidationParameters =
  new()
  {
    ValidIssuer = jwtSettings.Issuer,
    ValidAudience = jwtSettings.Audience,
    IssuerSigningKey = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(jwtSettings.Secret)
    ),
    ClockSkew = TimeSpan.Zero
  };

builder.Services.AddSingleton(tokenValidationParameters);

builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(
    options => options.TokenValidationParameters = tokenValidationParameters
  );
builder.Services.AddAuthorization(
  options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
      .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
      .RequireAuthenticatedUser()
      .Build()
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.SupportNonNullableReferenceTypes();

  var securityScheme = new OpenApiSecurityScheme
  {
    Description = "JWT Authorization header using the Bearer scheme",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = "bearer"
  };

  const string securitySchemeId = "bearerAuth";

  options.AddSecurityDefinition(securitySchemeId, securityScheme);

  options.AddSecurityRequirement(
    new OpenApiSecurityRequirement
    {
      {
        new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference
          {
            Type = ReferenceType.SecurityScheme,
            Id = securitySchemeId
          }
        },
        Array.Empty<string>()
      }
    }
  );
});
builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
  app.UseCors();

  using var scope = app.Services.CreateScope();
  var context =
    scope.ServiceProvider.GetService<ApplicationDbContext>()
    ?? throw new InvalidProgramException(
      $"{nameof(ApplicationDbContext)} should be registered"
    );
  context.Database.EnsureCreated();
}
else
{
  app.UseHsts();
  app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
