using Microsoft.EntityFrameworkCore;
using Radiocab.Models.SqlDb; // make sure this matches your actual namespace

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure database connection
builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Add MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ✅ Use session middleware (after building)
app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MyController1}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SqlDbContext>();
    db.Database.Migrate();

    if (!db.Users.Any(u => u.Role == "Admin"))
    {
        db.Users.Add(new Radiocab.Models.Users
        {
            Name = "System Admin",
            Email = "admin@radiocabs.in",
            Password = Radiocab.Models.PasswordHelper.HashPassword("admin123"),
            Role = "Admin",
            UnitType = "Admin"
        });
        db.SaveChanges();
    }
}

app.Run();
