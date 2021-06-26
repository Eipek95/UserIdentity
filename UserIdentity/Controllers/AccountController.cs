using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserIdentity.Identity;
using UserIdentity.Models;

namespace UserIdentity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        public AccountController()
        {
            var userStore = new UserStore<ApplicationUser>(new IdentityDataContext());
            userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new CustomPasswordValidator()//custom validator add
            {
                RequireDigit = true,
                RequiredLength = 7,//minimum char 
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonLetterOrDigit = true//alfanumerik değer
            };
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager) {
                RequireUniqueEmail = true,
                AllowOnlyAlphanumericUserNames = false//alphanumeric char içersin mi
            };
        }


        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)//kullanıcının gitmek istediği url ye yönlendirme yapsın
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.Find(model.Username, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Yanlış Kullanıcı Adı veya Parola");
                }
                else
                {
                    var authManager = HttpContext.GetOwinContext().Authentication;//authManager bizim uygulamadiki login işlemini yada logout işlemini yerine getirecek olan nesne
                    var identity = userManager.CreateIdentity(user, "ApplicationCookie");
                    var authProperties = new AuthenticationProperties()
                    {
                        IsPersistent = true//bu kısım için checkbox tan model gönderip beni hatırla olayını yapabiliriz.şimdilik varsayılan true
                    };
                    authManager.SignOut();//kullanıcı daha önceden giriş yapmışsa yani cookiesi varsa çıkarıldığından silindiğinden emin olmak için
                    authManager.SignIn(authProperties, identity);//signout işleminden sonra kullanıcıyı sisteme dahil etnej
                    return Redirect(string.IsNullOrEmpty(returnUrl) ? "/":returnUrl);//returnUrl boş ise ana dizine gönder
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);//kullanıcının girdiği bilgilerde hata varsa tekrar sayfaya yönlendirir ve kullanıcının girdiği bilgileri gözükür
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]//authorize olmayanlarında yani üye olmayanlarında girebildiği yerler
        public ActionResult Register(Register model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();
                user.UserName = model.Username;
                user.Email = model.Email;

                var result = userManager.Create(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);//key attribute register içereisindeki username,email veya passworddur.key yazmasak genel bir hata olur.hata mesajları ilgili view de ValidationSummary da çıkacaktır
                    }
                }
            }
            return View(model);
        }
       
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("Login");
        }
    }
}