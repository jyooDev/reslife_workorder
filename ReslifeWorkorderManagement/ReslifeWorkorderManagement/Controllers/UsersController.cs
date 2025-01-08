using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using ReslifeWorkorderManagement.Data;
using ReslifeWorkorderManagement.Models;
using System.Security.Claims;

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
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
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
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
