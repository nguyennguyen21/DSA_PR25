using DSA_PR25.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession();

    // Add services to the container.
    builder.Services.AddRazorPages();

    builder.Services.AddDbContext<ApplicationDBcontext>(options =>
        options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
        )
    );

    // Đăng ký xác thực cookie
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";             // Trang login
            options.AccessDeniedPath = "/Account/AccessDenied"; // Trang khi bị từ chối quyền
        });

    // Chính sách phân quyền
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
        options.AddPolicy("StudentOnly", policy => policy.RequireRole("student"));
    });

    builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Cho phép gửi ngay cả khi không có consent (cần thiết cho auth)
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
