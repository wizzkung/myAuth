using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myAuth.Abstraction;
using myAuth.Models;
using myAuth.Service;
using System.Security.Claims;

namespace myAuth.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        IService service;
        public AccountController(IService service)
        {
            this.service = service;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            var res = service.SignIn(new SignInRequest { login = "user1", psw = "1234" });
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(SignInRequest model)
        {
            var result = service.SignIn(model);
            if (result)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, model.login) };
                var identity = new ClaimsIdentity(claims,
                  CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
                return Redirect("~/Home/Index");
            }
            ModelState.AddModelError("", "Login and/or password is not correct");
            return View();
        }

        [HttpGet, AllowAnonymous]
        public IActionResult SignUp()
        {
            ViewData["GetRoles"] = service.GetRoles();
            return View();
        }

        [HttpPost, AllowAnonymous]
        public IActionResult SignUp(SignUpRequest model)
        {
            var result = service.SignUp(model);
            if (result)
            {
                //var claims = new[] { new Claim(ClaimTypes.Name, model.login) };
                //var identity = new ClaimsIdentity(claims,
                //  CookieAuthenticationDefaults.AuthenticationScheme);
                //HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                //    new ClaimsPrincipal(identity));
                return Redirect("/Account/Login");
            }
            ModelState.AddModelError("", "Login is already registered");
            ViewData["GetRoles"] = service.GetRoles();
            return View();
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePsw psw)
        {
            var result = service.ChangePassword(psw);
            if (result)
            {
                ViewData["Message"] = "Пароль успешно изменен!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Message"] = "Произошла ошибка, проверьте данные";
                return View();
            }
        }
    }
}
