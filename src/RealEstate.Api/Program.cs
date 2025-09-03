using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RealEstate.Application.Services;
using RealEstate.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RealEstate API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Bearer. Ej: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    var basePath = AppContext.BaseDirectory;
    foreach (var xml in Directory.GetFiles(basePath, "*.xml", SearchOption.TopDirectoryOnly))
        c.IncludeXmlComments(xml, includeControllerXmlComments: true);
});

var cs = builder.Configuration.GetConnectionString("SqlServer")
         ?? "Server=localhost;Database=RealEstateDb;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<RealEstateDbContext>(opt => opt.UseSqlServer(cs));

builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "THIS_IS_A_DEMO_SECRET_CHANGE_ME_1234567890";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateLifetime = true
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();