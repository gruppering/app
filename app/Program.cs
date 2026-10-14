using app.DAL;
using app.DTO.Course;
using app.Repositories;
using app.Services;
using app.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Loads environment variables from .env file.
if (File.Exists("../.env"))
{
    DotNetEnv.Env.Load("../.env");
}

// If .env does not exist, use placeholder values in example env file
else if (File.Exists("../example.env"))
{
    DotNetEnv.Env.Load("../example.env");
}

// Checks to see if we are inside a docker container,
// and if environment variables aren't set, defines them.
else if(EnvironmentUtils.IsInDocker())
{
    EnvironmentUtils.SetVariableIfNotPresent("DB_HOST", "postgres");
    
    EnvironmentUtils.SetVariableIfNotPresent("POSTGRES_PORT", "5432");
    
    EnvironmentUtils.SetVariableIfNotPresent("DB_NAME", "gruppering");
    
    EnvironmentUtils.SetVariableIfNotPresent("DB_USER", "grouper");
    
    EnvironmentUtils.SetVariableIfNotPresent("DB_PASSWORD", "Insecure");
}

// Adds DbContext and configures 
builder.Services.AddDbContextPool<GrupperingTypeContext>(options =>
{
    options.UseNpgsql($"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                      $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};" +
                      $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                      $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
                      $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};No Reset On Close=true;");
});

// Repositories and their associated services.
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<ICourseRepository,CourseRepository>();
builder.Services.AddScoped<CourseService>();

builder.Services.AddScoped<IGroupRepository,GroupRepository>();
builder.Services.AddScoped<GroupService>();


// Feature Flags
builder.Configuration["FeatureManagement:GoogleSignin"] = Environment.GetEnvironmentVariable("GOOGLE_SIGNIN_ENABLED"); // As we use a .env file, we set this manually, instead of having to update it in appsettings as well.
builder.Configuration["FeatureManagement:GitHubSignin"] = Environment.GetEnvironmentVariable("GOOGLE_SIGNIN_ENABLED"); // As we use a .env file, we set this manually, instead of having to update it in appsettings as well.

builder.Services.AddFeatureManagement();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

// Cookie auth configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
{
    opt.Cookie.Name = "GRUPPERING_SESSION";
    opt.LoginPath = "/login";
    opt.LogoutPath = "/logout";
    opt.Cookie.HttpOnly = true;
    opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    opt.Cookie.SameSite = SameSiteMode.Lax;
    opt.Cookie.IsEssential = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// A bit more elegant of a solution than manually running `dotnet ef database update` every time...
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GrupperingTypeContext>();
    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();

    ILoggerFactory factory = LoggerFactory.Create(logBuilder => logBuilder.AddConsole());
    var logger = factory.CreateLogger("Seeding");
    try
    {
        if (Environment.GetEnvironmentVariable("TEST_DB_SEEDING") == "true")
        {
            logger.LogInformation("Commencing Database Seeding");
            var courseService = scope.ServiceProvider.GetRequiredService<CourseService>();
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            var groupService = scope.ServiceProvider.GetRequiredService<GroupService>();

            var testAdminUser = await userService.Signup("adminTest", "admin.test@gruppering.no", "adminTest");
            var createdCourse = await courseService.CreateNewCourseAsync(
                new CourseCreationDto("eval", "This is for evaluation purposes", true), testAdminUser);

            var createdStudents = await userService.CreateRangeOfUsers("testUser", "gruppering.no", "test", 150);

            await courseService.AddUsersToCourseAsync(createdCourse.Id, createdStudents);

        }
    }
    catch(ArgumentException)
    {
        logger.LogWarning("Assuming seeding has already been done, skipping.");
    }
    
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCookiePolicy();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers().WithStaticAssets();


app.Run();