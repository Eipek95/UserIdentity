using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(UserIdentity.IdentityConfig))]

namespace UserIdentity
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            //kullanıcın tarayıcısına bir cookie bırakılacak
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType= DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath=new PathString("/Account/Login")//kullanıcı izni olmayan bir alana geldiği zaman account altındaki logine gönderecek
            });
        }
    }
}
