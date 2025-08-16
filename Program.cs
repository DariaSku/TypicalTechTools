using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TypicalTechTools.DataAccess;
using TypicalTechTools.Models.Data;
using TypicalTechTools.Models.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Ganss.Xss;
using TypicalTechTools.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Authentication/AdminLogin";
        option.AccessDeniedPath = "/Authentication/AccessDenied";

        option.ExpireTimeSpan = TimeSpan.FromMinutes(10);

        option.SlidingExpiration = true;
    });

//string connectionString = "";
//// Preprocessor directives to extract our configuration depending on whether we are debugging or deployed to production
//#if DEBUG
//// extract the connection string from appsettings.json
//connectionString = builder.Configuration.GetConnectionString("TypicalTechTools");
//#else
//// extract the connection string from an environment variable
//connectionString = Environment.GetEnvironmentVariable("TypicalTechTools");
//#endif
//// Use the relevant connection string
//builder.Services.AddDbContext<TypicalTechToolsDBContext>(c => c.UseSqlServer(connectionString));


// build the connection string from the one in appsettings.json
var connectionString = builder.Configuration.GetConnectionString("Default");

//link our application with our database
builder.Services.AddDbContext<TypicalTechToolsDBContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<HtmlSanitizer>();

builder.Services.AddScoped<EncryptionService>();
builder.Services.AddScoped<FileUploaderService>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton<CsvParser>();

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

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy",
                                 "default-src 'self'; " +
                                 "img-src 'self' https://picsum.photos; " +
                                 "script-src 'self' https://cdnjs.cloudflare.com;");

    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=63072000; includeSubDomains");

    await next();
});

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
