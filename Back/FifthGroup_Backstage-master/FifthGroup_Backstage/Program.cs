using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.AspNetCore.Http;
using FifthGroup_Backstage.ViewModel;
using FifthGroup_Backstage.Interfaces;
using FifthGroup_Backstage.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DbHouseContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("Data Source = dbHouse05.database.windows.net; Initial Catalog = dbHouse; Persist Security Info=True; User ID = Showshow306; Password=WaveF0219306")));
// Add session support
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1); // 将会话过期时间设置为1小时
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 設定HttpContextAccessor服務
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


// inject itaginterface
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IImageRepository, CloudinaryImageRepository>();

//inject Email服務
builder.Services.AddTransient<EmailService>();

//寄信Email服務
builder.Services.Configure<CEmailSettings>(builder.Configuration.GetSection("CEmailSettings"));
builder.Services.AddTransient<IEmailService, SEmailService>();








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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAuthorization();

app.MapControllerRoute(
    name: "email",
    pattern: "Email/CheckEmailContent/{id}",
    defaults: new { controller = "Email", action = "CheckEmailContent" });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Login}");

app.Run();