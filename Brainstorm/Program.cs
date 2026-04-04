using Brainstorm.DataAccess.Data;
using Brainstorm.DataAccess.Repository.IRepository;
using Brainstorm.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Brainstorm.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.ConfigureApplicationCookie(options =>//Cấu hình cookie xác thực ứng dụng.
{
    options.LoginPath = $"/Identity/Account/Login";//Đặt đường dẫn đến trang đăng nhập khi người dùng chưa xác thực.
    options.LogoutPath = $"/Identity/Account/Logout";//Đặt đường dẫn đến trang đăng xuất.
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";//Đặt đường dẫn đến trang từ chối truy cập khi người dùng không có quyền truy cập vào tài nguyên.
});

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
app.UseAuthentication();

app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Staff}/{controller=Home}/{action=Index}/{id?}");

app.Run();
