using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// DbContext 注入
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC
builder.Services.AddControllersWithViews();

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// HttpContextAccessor (供 Razor 判斷角色使用)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 錯誤頁
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // 開發環境暫時不要啟用 HTTPS 重導
    // app.UseHttpsRedirection();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection(); // 只在非開發環境啟用
}

// Middleware
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed Roles 與 Users
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!context.Role.Any())
    {
        context.Role.AddRange(
            new Role { Name = "Student" },
            new Role { Name = "Teacher" },
            new Role { Name = "Admin" }
        );
        context.SaveChanges();
    }

    if (!context.Users.Any())
    {
        context.Users.AddRange(
            new User { UserName = "s001", PasswordHash = "123" },
            new User { UserName = "t001", PasswordHash = "123" },
            new User { UserName = "admin", PasswordHash = "123" }
        );
        context.SaveChanges();
    }

    if (!context.UserRoles.Any())
    {
        var student = context.Users.First(u => u.UserName == "s001");
        var teacher = context.Users.First(u => u.UserName == "t001");
        var admin = context.Users.First(u => u.UserName == "admin");

        var studentRole = context.Role.First(r => r.Name == "Student");
        var teacherRole = context.Role.First(r => r.Name == "Teacher");
        var adminRole = context.Role.First(r => r.Name == "Admin");

        context.UserRoles.AddRange(
            new UserRole { UserId = student.UserId, RoleId = studentRole.Id },
            new UserRole { UserId = teacher.UserId, RoleId = teacherRole.Id },
            new UserRole { UserId = admin.UserId, RoleId = adminRole.Id }
        );
        context.SaveChanges();
    }
}

app.Run();
