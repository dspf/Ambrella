using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Ambrella.DataAccess;
using Ambrella.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using static AmbrellaWeb.Areas.Institution.Controllers.InstitutionsController;
using AmbrellaWeb.Areas.Landlord.Controllers;
using Ambrella.Models;
using static AmbrellaWeb.Areas.Inspector.Controllers.AssignmentsController;
using AmbrellaWeb.Areas.Inspector.Controllers;
using static AmbrellaWeb.Areas.Administrator.Controllers.InspectorController;
using AmbrellaWeb.Areas.Student.Controllers;
using AmbrellaWeb.Areas.Institution.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<AssignmentNotificationService>();
builder.Services.AddScoped<InspectorRecieveNotificationService>();
builder.Services.AddScoped<InstitutionNotificationService>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseDeveloperExceptionPage();

app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Administrator}/{controller=Home}/{action=Index}/{id?}");
app.MapLandlordRoutes(); // Register the Landlord area route
app.MapInspectorRoutes(); // Register the Inspector area route
app.MapStudentRoutes(); // Register the Inspector area route
app.MapInstitutionRoutes(); // Register the Inspector area route




app.Run();
