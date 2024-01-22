using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto();
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            ResponseDto? responseDto = await _authService.LoginAsync(model);

            if(responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", responseDto.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem(){ Text = SD.RoleCustomer, Value = SD.RoleCustomer }
            };

            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto model)
        {
            var roleList = new List<SelectListItem>()
                {
                    new SelectListItem(){ Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                    new SelectListItem(){ Text = SD.RoleCustomer, Value = SD.RoleCustomer }
                };

            ViewBag.RoleList = roleList;

            if (ModelState.IsValid)
            {
                ResponseDto result = await _authService.RegisterAsync(model);
                ResponseDto assignRole;

                if (result != null && result.IsSuccess)
                {
                    if (string.IsNullOrEmpty(model.Role))
                    {
                        model.Role = SD.RoleCustomer;
                    }
                    assignRole = await _authService.AssignRoleAsync(model);
                    if (assignRole != null && assignRole.IsSuccess)
                    {
                        TempData["success"] = "Registration Successful";
                        return RedirectToAction(nameof(Login));
                    }
                }
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Logput()
        {
            return View();
        }
    }
}
