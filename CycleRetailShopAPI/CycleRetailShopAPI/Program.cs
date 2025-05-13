using CycleRetailShopAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.Services;
using CycleRetailShopAPI.Repositories;
using CycleRetailShopAPI.Models;
using Stripe;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql; // Or another storage provider like Hangfire.PostgreSql



var builder = WebApplication.CreateBuilder(args);

// Read JWT settings directly from configuration
var jwtKey = builder.Configuration["JwtSettings:Key"];
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];
var expireMinutes = int.Parse(builder.Configuration["JwtSettings:ExpireMinutes"] ?? "60");


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Allow Angular App
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICycleService, CycleService>();
builder.Services.AddScoped<CycleRepository>();
// ? Register OrderService for IOrderService
builder.Services.AddScoped<ICustomerService, CycleRetailShopAPI.Services.CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<IUPIService, UPIService>();
// Register services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISalesReportService, SalesReportService>();

// Register background service
builder.Services.AddHostedService<ReportSchedulerService>();
builder.Services.AddScoped<ISalesReportService, SalesReportService>();
builder.Services.AddHostedService<ScheduledReportService>();
builder.Services.AddHostedService<ReportSchedulerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISalesReportService, SalesReportService>();





builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage(); // Or UsePostgreSqlStorage() if you're using PostgreSQL
});
builder.Services.AddHangfireServer();

builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});




// ?? Add Stripe settings configuration
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHangfireDashboard("/hangfire");

app.UseHangfireDashboard();
RecurringJob.AddOrUpdate<ScheduledReportService>(
    "daily-sales-report",
    service => service.SendReportsAsync("daily"),
    Cron.Daily);

RecurringJob.AddOrUpdate<ScheduledReportService>(
    "monthly-sales-report",
    service => service.SendReportsAsync("monthly"),
    "0 0 1 * *"); // First day of every month




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();
app.UseAuthentication(); // Ensure authentication is enabled
app.UseAuthorization();
app.MapControllers();
app.Run();
