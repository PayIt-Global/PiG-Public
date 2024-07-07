using PayEzPaymentApi.Helper;
using Microsoft.OpenApi.Models;
using PayItGlobal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using System.Configuration;
using PayItGlobal.Domain.Models;
using PayItGlobal.Infrastructure.Identity;
using PayItGlobal.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using PayItGlobal.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterServices();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDbContextPool<PayEzDb>(
   options => options.UsePostgreSql(builder.Configuration.GetConnectionString("PayEzConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UsePostgreSql(builder.Configuration.GetConnectionString("PayEzConnection")));

// Bind AppSettings section from appsettings.json to AppSettings class
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

// Register AppSettings as a singleton using IOptions pattern
builder.Services.AddSingleton<AppSettings>(sp =>
    sp.GetRequiredService<IOptions<AppSettings>>().Value);

builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PayEz Payment Api enabled with JWT Bearer",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Secret"] ?? throw new InvalidOperationException("Secret is not configured in AppSettings"))),

            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ClockSkew = TimeSpan.Zero // Optional: reduce or eliminate clock skew allowance
        };
    });


builder.Services.AddAuthorization(); // Adds authorization services
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Ensures controllers are routed
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();


//app.UseMiddleware<DiagnosticMiddleware>();
app.UseMiddleware<CustomLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var appSettings = context.RequestServices.GetRequiredService<IOptions<AppSettings>>();
    // Adjusted to use UserManager<ApplicationUser>
    var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
    var jwtMiddleware = new JwtMiddleware(next, appSettings, userManager);
    await jwtMiddleware.Invoke(context, context.RequestServices.GetService<IUnitOfWork>(), context.RequestServices);
});

app.MapControllers();
app.Run();

