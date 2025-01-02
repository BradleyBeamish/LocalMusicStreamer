using LocalMusicStreamer.Models;
using LocalMusicStreamer.Utilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

try
{
    // Add DbContext service to the DI container
    builder.Services.AddDbContext<LocalMusicStreamerContext>(options =>
        options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            new MySqlServerVersion(new Version(10, 11, 10)) // MariaDB 10.11.10
        ));
}
catch (Exception ex)
{
    Console.WriteLine("Could not connect to database: " + ex.Message);
    Environment.Exit(1);
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LocalMusicStreamerContext>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

    // Uncomment to sync local songs with DB songs
    //DatabaseSeeder.Seed(context, env);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    // Fixes Flac Files not Playing
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads")),
    RequestPath = "/uploads",
    ServeUnknownFileTypes = true,
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();