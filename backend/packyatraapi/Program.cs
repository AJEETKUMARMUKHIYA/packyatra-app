using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Data;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services;
using MoversAndPackerApi.Services.Interfaces;
using PackersAndMoversAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<TwilioCredentials>(builder.Configuration.GetSection("TwilioCredentials"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();// Swagger added 
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<PriceService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<TimeSlotService>();
// Change from:
builder.Services.AddScoped<DashboardService>();

// To:
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddScoped<BhashSmsService>();
builder.Services.AddHttpClient<WhatsAppService>();
builder.Services.AddScoped<ISendGridEmailService, SendGridEmailService>();
builder.Services.AddSingleton<Fast2smsService>();
builder.Services.AddScoped<FAQService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

builder.Services.AddSingleton(new OTPService(
    user: "your_username",
    pass: "your_password",
    sender: "SENDERID"
));
builder.Services.AddScoped<AdminUserService>();
builder.Services.AddSingleton<GenAIService>();
builder.Services.AddScoped<PricingService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<TicketAttachmentService>();
builder.Services.AddScoped<ITicketTrackingService, TicketTrackingService>();
builder.Services.AddScoped<IPhonePeService, PhonePeService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Serves Swagger at the root URL
    });
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowAll");  // MUST come immediately after UseRouting() and BEFORE MapControllers()

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
