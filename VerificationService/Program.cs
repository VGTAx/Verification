using Microsoft.EntityFrameworkCore;
using VerificationApp.Data;
using VerificationApp.Middlewares;
using VerificationApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<VerificationAppContext>(options =>
  options.UseMySql(builder.Configuration.GetConnectionString("ConnectionString"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ConnectionString"))));
builder.Services.AddTransient<ExcelService>();
builder.Services.AddTransient<ErrorHandlerMiddleware>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "details",
    pattern: "Verification/Details/{id}/{returnUrl?}",
    defaults: new { controller = "Verification", action = "Details" });
app.MapControllerRoute(
    name: "edit",
    pattern: "Verification/Details/{id}/{returnUrl?}",
    defaults: new { controller = "Verification", action = "Edit" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Verification}/{action=Index}/{id?}");



app.Run();
