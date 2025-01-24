using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReslifeWorkorderManagement.Data;
using ReslifeWorkorderManagement.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Database connection
var connectionString = builder.Configuration.GetConnectionString("default");

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(connectionString)
    );

// Configure identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings. 
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;

    // Lockout settings. 
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings. 
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});


builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Users/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Create roles
using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Administrator", "StudentSupervisor", "MaintenanceStaff", "DeskStaff" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}


// For Development Env, start the app with an admin account
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string email = "testadmin@admin.com";
    string username = "testadmin";
    string password = "Testadmin123";
    string firstName = "Admin";
    string lastName = "Lee";

    // Ensure the role "Administrator" exists
    if (!await roleManager.RoleExistsAsync("Administrator"))
    {
        await roleManager.CreateAsync(new IdentityRole("Administrator"));
    }

    // Check if the user already exists
    if (await userManager.FindByNameAsync(username) == null)
    {
        var user = new ApplicationUser()
        {
            UserName = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Add the user to the "Administrator" role
            await userManager.AddToRoleAsync(user, "Administrator");
            Console.WriteLine($"User '{username}' created and added to role 'Administrator'.");
        }
        else
        {
            // Log errors
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error: {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine($"User '{username}' already exists.");
    }
}


app.Run();
