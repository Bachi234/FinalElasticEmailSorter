using automationTest.Context;
using automationTest.Models;
using automationTest.Service;
using automationTest.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddDbContext<ElasticDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ElasticDB")));

builder.Services.AddDbContext<MktgDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("MktgDB")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ElasticDB")));

builder.Services.AddDbContext<ApplicationDbContext_>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("MktgDB")));



builder.Services.AddScoped<EventDataService>();
builder.Services.AddScoped<ElasticDataService>();
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<ApplicationDbContext_>();
builder.Services.AddScoped<MktgDbContext>();
builder.Services.AddScoped<ElasticDbContext>();

builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=DisplayElasticData}/{id?}");
   
app.Run();
