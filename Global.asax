using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using StudentManagement.Models; // 請改成你自己的 Models 命名空間

namespace StudentManagement
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // 初始化角色與管理員帳號
            SeedRolesAndAdmin();
        }

        private void SeedRolesAndAdmin()
        {
            using (var context = new ApplicationDbContext())
            {
                // 1️⃣ 初始化角色
                if (!context.Roles.Any(r => r.Name == "Student"))
                    context.Roles.Add(new Role { Name = "Student" });

                if (!context.Roles.Any(r => r.Name == "Teacher"))
                    context.Roles.Add(new Role { Name = "Teacher" });

                if (!context.Roles.Any(r => r.Name == "Admin"))
                    context.Roles.Add(new Role { Name = "Admin" });

                context.SaveChanges();

                // 2️⃣ 初始化預設管理員帳號
                if (!context.Users.Any(u => u.UserName == "admin"))
                {
                    var adminUser = new User
                    {
                        UserName = "admin",
                        Password = "123456" // 建議正式專案使用 Hash 加密
                    };
                    context.Users.Add(adminUser);
                    context.SaveChanges();

                    // 3️⃣ 將管理員加入 Admin 角色
                    var adminRole = context.Roles.First(r => r.Name == "Admin");
                    context.UserRoles.Add(new UserRole
                    {
                        UserId = adminUser.Id,
                        RoleId = adminRole.Id
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
