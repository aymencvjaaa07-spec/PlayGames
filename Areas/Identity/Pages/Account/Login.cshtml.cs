using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayGames.Models;

namespace PlayGames.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ReturnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
            return Page();

        var email = Input.Email.Trim();
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "The email or password you entered is incorrect.");

            return Page();
        }

        // التحقق الحقيقي من كلمة المرور
        var passwordCorrect =
            await _userManager.CheckPasswordAsync(user, Input.Password);

        if (!passwordCorrect)
        {
            ModelState.AddModelError(
                string.Empty,
                "The email or password you entered is incorrect.");

            return Page();
        }

        // تسجيل الدخول بدون Lockout
        await _signInManager.SignInAsync(user, Input.RememberMe);

        return LocalRedirect(ReturnUrl);
    }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
