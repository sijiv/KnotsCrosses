using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using My.SignalRTest.Models;
using System.Security.Claims;

namespace My.SignalRTest
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                //ClientId = "864274910122-l8lsv7m6bjj2au19647v54pvea2vq8oe.apps.googleusercontent.com",
                //ClientSecret = "CbdBKcxjG56ey5KipTj1b6KE",
                ClientId = "864274910122-25vvr19q1l5cn3iokapvftv9mtpjujr4.apps.googleusercontent.com",
                ClientSecret = "stwwxHjbnWBgPVcPVjhETBRA",
            });
            //var httpSession = System.Web.HttpContext.Current.Session;
            //var googleOAuth2AuthenticationOptions = new GoogleOAuth2AuthenticationOptions()
            //           {
            //               ClientId = "864274910122-l8lsv7m6bjj2au19647v54pvea2vq8oe.apps.googleusercontent.com",
            //               ClientSecret = "CbdBKcxjG56ey5KipTj1b6KE",
            //               Provider = new GoogleOAuth2AuthenticationProvider()
            //               {
            //                   OnAuthenticated = async context =>
            //                   {
            //                       httpSession.Add("pictureUrl", context.User.GetValue("picture").ToString());
            //                       //context.Identity.AddClaim(new Claim("picture", context.User.GetValue("picture").ToString()));
            //                       //context.Identity.AddClaim(new Claim("profile", context.User.GetValue("profile").ToString()));
            //                   }
            //               }
            //           };
            ////googleOAuth2AuthenticationOptions.Scope.Add("email");
            //app.UseGoogleAuthentication(googleOAuth2AuthenticationOptions);

            app.MapSignalR();
        }
    }
}