using FifthGroup_front.Models;
using FifthGroup_front.Filter;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using FifthGroup_front.ViewModels;
using FifthGroup_front.Interfaces;
using FifthGroup_front.Services;
using FifthGroup_front.Hubs;
using FifthGroup_Backstage.Repositories;
using FifthGroup_front.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPasswordHasher<Resident>, PasswordHasher<Resident>>();

builder.Services.AddDbContext<DbHouseContext>(options =>
{
    // 假設你使用的是SQL Server
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddRazorPages();

// Add  services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AuthenticationFilter>();
});


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


var configuration = builder.Configuration;

builder.Services.Configure<BEmailViewModel>(builder.Configuration.GetSection(BEmailViewModel.Name));
builder.Services.AddSingleton<EmailService>();

// Configure email service
builder.Services.Configure<CEmailSettings>(builder.Configuration.GetSection("CEmailSettings"));
builder.Services.AddTransient<IEmailService, SEmailService>();

// inject itaginterface
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IImageRepository, CloudinaryImageRepository>();

//加入 SignalR
builder.Services.AddSignalR();


var app = builder.Build();
app.UseSession();

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

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//加入 Hub
app.MapHub<ChatHub>("/chatHub");


app.Run();
