using MICHO.API.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("MICHOAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7249/api/"); // base URL cho API
});
builder.Services.AddDbContext<Prn232Pt2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBContext")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
