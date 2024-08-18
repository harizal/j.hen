
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WApp;
using WApp.Datas;
using WApp.Helpers;
using WApp.Models;
using WApp.Utlis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDataContext>(option => option.UseSqlite(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddIdentity<AppUser, IdentityRole>(
    option =>
    {
        option.Password.RequiredUniqueChars = 0;
        option.Password.RequireUppercase = false;
        option.Password.RequireLowercase = false;
        option.Password.RequiredLength = 8;
        option.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AppDataContext>().AddDefaultTokenProviders();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var userManager = serviceProvider.GetService<UserManager<AppUser>>();
    var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

    await SeedHelper.SeedUserAndRoles(userManager, roleManager);
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseExceptionHandler();

app.Run();
