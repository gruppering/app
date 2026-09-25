using app.DAL;
using app.Repositories;
using app.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Loads environment variables from .env file.
if (File.Exists("../.env"))
{
    DotNetEnv.Env.Load("../.env");
}
else // Then assumes the .env file isn't present, but the Postgres container is running, therefore trying the same default credentials as in the compose files.
{
    Environment.SetEnvironmentVariable("DB_HOST","localhost");
    Environment.SetEnvironmentVariable("POSTGRES_PORT","5455");
    Environment.SetEnvironmentVariable("DB_NAME","gruppering");
    Environment.SetEnvironmentVariable("DB_USER","grouper");
    Environment.SetEnvironmentVariable("DB_PASSWORD","Insecure");
}

// Adds DbContext and configures 
builder.Services.AddDbContextPool<GrupperingTypeContext>(options =>
{
    options.UseNpgsql($"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                      $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};" +
                      $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                      $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
                      $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};");
});

builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<ICourseRepository,CourseRepository>();
builder.Services.AddScoped<CourseService>();

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

app.UseHttpsRedirection();
app.UseRouting();

app.UseCookiePolicy();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers().WithStaticAssets();


app.Run();