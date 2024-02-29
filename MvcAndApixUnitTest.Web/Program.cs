using Microsoft.EntityFrameworkCore;
using MvcAndApixUnitTest.Web.Models;
using MvcAndApixUnitTest.Web.Repository;

var builder = WebApplication.CreateBuilder(args);

/*Scaffold-DbContext "Server URL" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models*/

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<XUnitTestDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectStr"));
});
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
