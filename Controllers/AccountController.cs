using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using StudentManagement.Models;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace StudentManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string userName, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "帳號與密碼不可為空");
                return View();
            }

            string passwordHash = ComputeSha256Hash(password);

            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == userName && u.PasswordHash == passwordHash);

            if (user == null)
            {
                ModelState.AddModelError("", "帳號或密碼錯誤");
                return View();
            }

            // 建立 Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = user.UserRoles.Select(ur => ur.Role.Name);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Roles = new List<string> { "Student", "Teacher", "Admin" };
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string userName, string password, string role, string name, string email)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("", "帳號、密碼、角色與姓名皆為必填");
                ViewBag.Roles = new List<string> { "Student", "Teacher", "Admin" };
                return View();
            }

            // 檢查帳號是否已存在
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "此帳號已存在");
                ViewBag.Roles = new List<string> { "Student", "Teacher", "Admin" };
                return View();
            }

            string passwordHash = ComputeSha256Hash(password);

            // 建立新使用者
            var newUser = new User
            {
                UserName = userName,
                PasswordHash = passwordHash,
                Name = name,
                Email = email,
                CreatedAt = DateTime.Now
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // 對應角色
            var userRole = await _context.Role.FirstOrDefaultAsync(r => r.Name == role);
            if (userRole != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = newUser.UserId,
                    RoleId = userRole.Id
                });
                await _context.SaveChangesAsync();
            }

            // 註冊完成後直接登入
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, newUser.UserName),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/IsUserNameAvailable?userName=xxx
        [HttpGet]
        public async Task<JsonResult> IsUserNameAvailable(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return Json(false);

            var exists = await _context.Users.AnyAsync(u => u.UserName == userName);
            return Json(!exists);
        }

        // SHA256 加密密碼
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
