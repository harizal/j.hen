using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using WApp.Datas;
using WApp.Helpers;
using WApp.Models;
using WApp.Utlis;
using WApp.ViewModels;
using WApp.ViewModels.Parameters;

namespace WApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppDataContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(ILogger<AccountController> logger, AppDataContext context, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;

        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                Alert(Constans.Label.InvalidLogin, Enums.NotificationType.info);
                return View(model);
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Username
                };

                var existingUser = await _userManager.FindByNameAsync(user.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", string.Format(Constans.Label.AlreadyExists, "Username"));
                    return View(model);
                }

                existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", string.Format(Constans.Label.AlreadyExists, "Email"));
                    return View(model);
                }


                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Name", model.Name));

                    await _userManager.AddToRoleAsync(user, Constans.RoleUser);
                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetUsers(BaseDTParameters param)
        {
            var results = new List<UserViewModel>();

            if (param.AdditionalValues != null && param.AdditionalValues.Any())
            {
                var filters = param.AdditionalValues.ToList();

                var query = from user in _userManager.Users
                            join userRole in _context.UserRoles on user.Id equals userRole.UserId
                            join role in _context.Roles on userRole.RoleId equals role.Id
                            where user.Name.Contains(filters[0] ?? string.Empty) && user.Email.Contains(filters[1] ?? string.Empty)
                            select new UserViewModel
                            {
                                Id = user.Id,
                                Email = user.Email,
                                Name = user.Name,
                                Role = role.Name,
                            };
                            

                results = await query.ToListAsync();
            }

            return new JsonResult(DataTablePagedHelper.GetDatatablePaged(results, param));
        }

        private UserViewModel SetRoles(UserViewModel model)
        {
            model.Roles = [
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "", Value = "" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = Constans.RoleUser, Value = Constans.RoleUser },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = Constans.RoleAdministrator, Value = Constans.RoleAdministrator }
                ];
            return model;
        }

        [Authorize]
        public IActionResult Create()
        {
            var model = new UserViewModel();
            return View(SetRoles(model));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingUser = await _userManager.FindByNameAsync(model.Username);
                    if (existingUser != null)
                    {
                        Alert(string.Format(Constans.Label.AlreadyExists, "Username"), Enums.NotificationType.error);
                        return View(SetRoles(model));
                    }

                    existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        Alert(string.Format(Constans.Label.AlreadyExists, "Email"), Enums.NotificationType.error);
                        return View(SetRoles(model));
                    }
                    var user = new AppUser
                    {
                        UserName = model.Username,
                        Email = model.Email,
                        Name = model.Name
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);

                        Alert(Constans.Label.SavedSuccess, Enums.NotificationType.success);
                        return RedirectToAction("Index", "Account");
                    }
                }
                else
                {
                    var message = string.Empty;
                    foreach (var item in ModelState.Where(m => m.Value.ValidationState != Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid))
                    {
                        message += Environment.NewLine + item.Value.Errors[0].ErrorMessage;
                    }
                    Alert(message, Enums.NotificationType.error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");

                Alert(ex.Message, Enums.NotificationType.error);
            }
            return View(SetRoles(model));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var existingUser = _userManager.Users.FirstOrDefault(m => m.Id == id);
            if (existingUser == null)
                return Ok(new { isSuccess = false, error = Constans.Label.DataNotFound });
            else
            {
                if(existingUser.UserName == User.Identity?.Name)
                    return Ok(new { isSuccess = false, error = Constans.Label.CanntDeleteCurrentUser });
                if (existingUser.UserName == Constans.EmailUserDefault)
                    return Ok(new { isSuccess = false, error = Constans.Label.CanntDeleteSuperAdmin });

                await _userManager.DeleteAsync(existingUser);
                return Ok(new { isSuccess = true });
            }
        }
    }
}
