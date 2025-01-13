using ClosedXML.Excel;
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

        [HttpGet, AllowAnonymous]
        public IActionResult Report()
        {
            return View();
        }

        [HttpGet, Route("GetExcel"), AllowAnonymous]
        public ActionResult GetExcel()
        {
            using (var ms = new MemoryStream())
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.AddWorksheet("report");
                    ws.Cell(1, 1).Value = "Id";
                    ws.Cell(1, 1).Style.Font.Bold = true;
                    ws.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(1, 2).Value = "Name";
                    ws.Cell(1, 2).Style.Font.Bold = true;
                    ws.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    //ws.Column(1).Width = 25;
                    //ws.Column(2).Width = 15;

                    //List<Student> lst = new List<Student>()
                    //{
                    //    new Student{Id=1, Name="Иванов" },
                    //    new Student{Id=2, Name="Петров" }
                    //};

                    //ws.Cell(2, 1).InsertData(lst);
                    //ws.Cell(2, 1).InsertData(null);
                    ws.RangeUsed().SetAutoFilter();
                    ws.Columns("A", "B").AdjustToContents();

                    ws.SheetView.FreezeRows(1);
                    wb.SaveAs(ms);
                    ms.Position = 0;
                    ms.Flush();
                    var bytes = ms.ToArray();

                    return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "report____" + DateTime.Now.ToString("ddMMyyyy_hhmmss") + ".xlsx");
                }
            }
        }

        [HttpGet, Route("GetCsv"), AllowAnonymous]
        public IActionResult ExportToCSV(IEnumerable<RoleResponse> roles)
        {
            string csvContent = string.Empty;

       
            csvContent += "RoleId,RoleName" + Environment.NewLine;

            
            foreach (var role in roles)
            {
                csvContent += $"{role.id},{role.name}" + Environment.NewLine;
            }

            
            var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
            return File(bytes, "application/octet-stream", "RoleData.csv");
        }



    }
}
