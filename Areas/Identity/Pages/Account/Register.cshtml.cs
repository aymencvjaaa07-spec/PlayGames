using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayGames.Models;

namespace PlayGames.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager){_userManager=userManager;_signInManager=signInManager;}
    [BindProperty] public InputModel Input {get;set;}=new();
    [BindProperty(SupportsGet=true)] public string? ReturnUrl {get;set;}
    public void OnGet(){}
    public async Task<IActionResult> OnPostAsync(){
        ReturnUrl ??= Url.Content("~/");
        if(!ModelState.IsValid)return Page();
        var email=Input.Email.Trim(); var userName=Input.UserName.Trim();
        if(await _userManager.FindByEmailAsync(email)!=null){ModelState.AddModelError(nameof(Input.Email),"This email address is already registered.");return Page();}
        if(await _userManager.FindByNameAsync(userName)!=null){ModelState.AddModelError(nameof(Input.UserName),"This username is already taken.");return Page();}
        var user=new ApplicationUser{UserName=userName,Email=email,EmailConfirmed=false};
        var result=await _userManager.CreateAsync(user,Input.Password);
        if(result.Succeeded){await _signInManager.SignInAsync(user,false);return LocalRedirect(ReturnUrl);}
        foreach(var error in result.Errors)ModelState.AddModelError(string.Empty,error.Description);
        return Page();
    }
    public class InputModel{[Required,StringLength(32,MinimumLength=3)]public string UserName{get;set;}=string.Empty;[Required,EmailAddress]public string Email{get;set;}=string.Empty;[Required,StringLength(100,MinimumLength=1),DataType(DataType.Password)]public string Password{get;set;}=string.Empty;[Required,DataType(DataType.Password),Compare(nameof(Password))]public string ConfirmPassword{get;set;}=string.Empty;}
}


