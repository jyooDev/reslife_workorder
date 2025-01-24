using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using ReslifeWorkorderManagement.Data;
using ReslifeWorkorderManagement.Models;
using System.Globalization;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReslifeWorkorderManagement.Controllers
{
    public class UsersController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;
        private readonly ApplicationDbContext _context;

        public UsersController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<UsersController> logger, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = new Dictionary<string, List<StaffVM>>
            {
                { "Administrators", new List<StaffVM>() },
                { "StudentSupervisors", new List<StaffVM>() },
                { "MaintenanceStaffs", new List<StaffVM>() },
                { "DeskStaffs", new List<StaffVM>() }
            };

            var userroles = await _context.UserRoles.ToListAsync();
            var roles = await _context.Roles.ToListAsync();
            foreach(ApplicationUser user in _userManager.Users)
            {
                var role = userroles.Where(ur => ur.UserId == user.Id)
                    .Join(roles, userrole => userrole.RoleId, role => role.Id, (userrole, role) => role.Name).FirstOrDefault();
                if(role != null)
                {
                    StaffVM staff = new StaffVM()
                    {
                        user = user,
                        role = role
                    };

                    switch (role)
                    {
                        case "Administrator":
                            model["Administrators"].Add(staff);
                            break;
                        case "StudentSupervisor":
                            model["StudentSupervisors"].Add(staff);
                            break;
                        case "MaintenanceStaff":
                            model["MaintenanceStaffs"].Add(staff);
                            break;
                        default:
                            model["DeskStaffs"].Add(staff);
                            break;
                    }
                }
            }
            return View(model);
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username!, model.Password!, false, false);

                if (!result.Succeeded)
                {
                    _logger.LogError(result.ToString());
                    if (result.ToString().Equals("Failed"))
                    {
                        ModelState.AddModelError("", "UserName or password is incorrect.");
                    }
                    else if (result.ToString().Equals("NotAllowed"))
                    {
                        ModelState.AddModelError("", "The user email needs confirmation.");
                    }
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                var claims = new List<Claim>
                {
                    new Claim("firstname", user.FirstName),
                    new Claim("lastname", user.LastName)
                };
                await _signInManager.SignInWithClaimsAsync(user, false, claims);
                return RedirectToAction("Index", "Dashboard");
            }
            return View(model);
        }


        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles="Administrator")]
        [HttpGet]
        public async Task<IActionResult> Create(string role)
        {
            UserCreateVM model = new UserCreateVM()
            {
                FirstName = null,
                LastName = null,
                Email = null,
                Password = null,
                Role = role
            };
            return PartialView("_CreateUserPartial", model);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateVM newUser)
        {
            // Return the modal with validation error
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateUserPartial", newUser);
            }

            // Check if the username is already in use.
            var existingEmail = await _userManager.FindByEmailAsync(newUser.Email!);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "This email is already in use. Please choose another.");
                return PartialView("_CreateUserPartial", newUser);

            }

            // Format the name to title case
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            var formattedFirstName = textInfo.ToTitleCase(newUser.FirstName!.ToLower());
            var formattedLastName = textInfo.ToTitleCase(newUser.LastName!.ToLower());

            // Create user object
            var user = new ApplicationUser()
            {
                UserName = newUser.Email,
                Email = newUser.Email,
                FirstName = formattedFirstName,
                LastName = formattedLastName,
                EmailConfirmed = true
            };


            // Add user to DB
            var result = await _userManager.CreateAsync(user, newUser.Password!);
            _logger.LogInformation(result.ToString());

            // If failed adding user, pass error alert to the frontend.
            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Something went wrong while adding the user. Try again." });
            }

            _logger.LogInformation("User is created with an initial random password.");


            // Add role to user
            var a = await _userManager.AddToRoleAsync(user, newUser.Role!);

            return Json(new { success = true, message = "User is added succesfully." });
        }


        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return PartialView("_DeleteUserPartial", user);
        }


        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var result = _userManager.DeleteAsync(user);
            if (!result.Result.Succeeded)
            {
                return Json(new { success = false, message = "Something went wrong while deleting the user. Try again." });
            }
            return Json(new { success = true, message = "User is deleted successfully." });
        }


        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userroles = await _context.UserRoles.ToListAsync();
            var roles = await _context.Roles.ToListAsync();
            var role = userroles.Where(ur => ur.UserId == user.Id)
                    .Join(roles, userrole => userrole.RoleId, role => role.Id, (userrole, role) => role.Name).FirstOrDefault();

            UserEditVM model = new UserEditVM()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                OldPassword = null,
                NewPassword = null,
                Role = role
            };
            return View("Edit", model);
        }


        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }


            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.Role);

            if (!string.IsNullOrWhiteSpace(model.OldPassword) && !string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            return RedirectToAction("Index", "Users");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }


    }
}
