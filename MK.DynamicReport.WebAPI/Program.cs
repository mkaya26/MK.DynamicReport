using MK.DynamicReport.Application.Interfaces;
using MK.DynamicReport.Domain.Interfaces;
using MK.DynamicReport.Infrastructure.Repositories;
using MK.DynamicReport.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Hangfire;
using MK.DynamicReport.Domain.Settings;
using SixLabors.ImageSharp;
using System.Net.Mail;
using System.Net;
using Microsoft.IdentityModel.Abstractions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportDefinitionRepository, ReportDefinitionRepository>();
builder.Services.AddScoped<IReportHistoryRepository, ReportHistoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGraphicsService, GraphicsService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddTransient<ReportBackgroundJobService>();
// 
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new Hangfire.SqlServer.SqlServerStorageOptions
          {
              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
              QueuePollInterval = TimeSpan.FromSeconds(15),
              UseRecommendedIsolationLevel = true,
              DisableGlobalLocks = true
          }));
builder.Services.AddHangfireServer();

var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

builder.Services
    .AddFluentEmail(smtpSettings.Username)
    .AddRazorRenderer()
    .AddSmtpSender(() => new SmtpClient
    {
        Host = smtpSettings.Host,
        Port = smtpSettings.Port,
        Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
        EnableSsl = true
    });



var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(setup =>
{
    var jwtSecuritySheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** yourt JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecuritySheme.Reference.Id, jwtSecuritySheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecuritySheme, Array.Empty<string>() }
                });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<ReportBackgroundJobService>(
    "rapor-export-email-job",
    job => job.ExportReportAndSendEmailAsync(1, "kaya.esk@gmail.com"),
    Cron.Daily,
    TimeZoneInfo.Local);


app.UseHttpsRedirection();

app.UseAuthentication();  
app.UseAuthorization();    

app.MapControllers();

app.Run();
