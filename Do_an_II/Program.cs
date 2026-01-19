using Do_an_II.Data;
using Do_an_II.DbInitializer;
using Do_an_II.Hubs;
using Do_an_II.Repository;
using Do_an_II.Repository.IRepository;
using Do_an_II.Services.ChatServices;
using Do_an_II.Services.EmailServices;
using Do_an_II.Services.RabbitMQServices;
using Do_an_II.Services.RedisServices;
using Do_an_II.Services.VnPay;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "Do_an_II_";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("RedisConnection");
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(
    options => {
        options.SignIn.RequireConfirmedAccount = true;
        //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //options.Lockout.MaxFailedAccessAttempts = 5;
        //options.Lockout.AllowedForNewUsers = true;
    }
    ).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddHostedService<FlashSaleOrderConfirmedConsumer>();
builder.Services.AddHostedService<FlashSaleOrderConsumer>();
builder.Services.AddScoped<OrderPublisher>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
});

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IEmailSender,EmailSender>();
builder.Services.AddScoped<IChatService,ChatService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSignalR();
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
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
using (var scope = app.Services.CreateScope())
{
    var mqConn = scope.ServiceProvider.GetRequiredService<RabbitMqConnection>();
    var connection = await mqConn.CreateConnectionAsync();

    try
    {
        await RabbitMqInitializer.InitAsync(connection);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to initialize RabbitMQ: {ex.Message}");
        throw;
    }
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapHub<ChatHub>("/ChatHub");
app.MapHub<DashboardHub>("/DashboardHub");
app.MapHub<NotifyHub>("/NotifyHub");
app.MapHub<OrderTrackingHub>("/OrderTrackingHub");

SeedDatabase();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}