using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using EstudoOtel.Telemetry;

namespace EstudoOtel.Controllers
{
    public class LoginController(UsuarioMetrica usuarioMetrica) : Controller
    {
        // GET: /Login
        [HttpGet]
        public IActionResult Index(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Replace this with your real credential validation.
            // For demo: accept any non-empty username.
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            // Create user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.NameIdentifier, model.Email)
            };

            // Use the cookie scheme registered in Program.cs ("CookieAuth")
            var identity = new ClaimsIdentity(claims, "EstudoOtel.Cookie");
            var principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                RedirectUri = model.ReturnUrl
            };

            await HttpContext.SignInAsync("EstudoOtel.Cookie", principal, props);

            // track logged user (optional)
            usuarioMetrica.AtualizarUsuario(model.Email);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        // GET: /Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("EstudoOtel.Cookie");

            // Remove from logged users dictionary (optional)
            
            if (User?.Identity?.IsAuthenticated == true)
            {
                var user = User?.Identity?.Name;
                if(!string.IsNullOrEmpty(user))
                {
                    usuarioMetrica.RemoverUsuario(user!);
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }

    // Simple view model for login form
    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}