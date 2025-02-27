using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Handlers;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Database
builder.Services.AddDbContext<FoodDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o => o.UseNetTopologySuite()));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;  // If testing on HTTP, Disable HTTPS for testing purposes
        options.SaveToken = true; // Saves token in the authentication context

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,  // Ensures token comes from a valid Issuer
            ValidateAudience = true,  // Ensures token is intended for the right audience
            ValidateLifetime = true,  // Ensures token is not expired
            ValidateIssuerSigningKey = true,  // Ensures token is signed with a valid key
            ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Reads Issuer from appsettings.json
            ValidAudience = builder.Configuration["Jwt:Audience"],  // Reads Audience from appsettings.json
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Uses the secret key to verify the token's signature
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

// Enforce Role-Based Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOnly", policy => policy.RequireRole("Vendor"));
    options.AddPolicy("CompanyOnly", policy => policy.RequireRole("Company"));
    options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
});

// Add Controllers with Global Authorization
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter()); // Enforce global authentication
});

// Add Swagger (With Authentication Support)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "OfficeFoodAPI", Version = "v1" });

    // Include XML comments in Swagger
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);

    // Enable JWT Auth in Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your_token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Register Handlers & Services
builder.Services.AddScoped<UserAuthHandler>();

var app = builder.Build();

// Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();  // Ensure authentication is applied
app.UseAuthorization();   // Ensure authorization is enforced
app.MapControllers();
app.Run();
