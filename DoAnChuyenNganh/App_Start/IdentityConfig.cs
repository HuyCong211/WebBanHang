using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using DoAnChuyenNganh.Models;
using System.Net;
using System.Net.Mail;

namespace DoAnChuyenNganh
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            try
            {
                var mail = new System.Net.Mail.MailMessage();
                mail.To.Add(message.Destination);
                mail.From = new System.Net.Mail.MailAddress("krikshop.dacn.n2@gmail.com", "KrikShop Support");
                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml = true;

                using (var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential("krikshop.dacn.n2@gmail.com", "cafl dmri cnuq zdqb");
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }

                System.Diagnostics.Debug.WriteLine("✅ GỬI MAIL THÀNH CÔNG ĐẾN: " + message.Destination);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ SMTP ERROR: " + ex.ToString());
                throw;
            }


            // Gửi mail bằng Gmail
            //var mail = new System.Net.Mail.MailMessage();
            //mail.To.Add(message.Destination);
            //mail.From = new System.Net.Mail.MailAddress("krikshop.dacn.n2@gmail.com");
            //mail.Subject = message.Subject;
            //mail.Body = message.Body;
            //mail.IsBodyHtml = true;

            //var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
            //{
            //    Credentials = new System.Net.NetworkCredential("krikshop.dacn.n2@gmail.com", "cafl dmri cnuq zdqb"),
            //    EnableSsl = true
            //};

            //return smtp.SendMailAsync(mail);
            //// Plug in your email service here to send an email.
            //return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            // Mặc định cho cookie khách hàng
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager, "CustomerCookie");

        }



        //public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        //{
        //    // Nếu chưa có HttpContext (khi login lần đầu) → mặc định là CustomerCookie
        //    var authType = "CustomerCookie";

        //    try
        //    {
        //        // Nếu đang trong request (ví dụ admin đăng nhập)
        //        if (HttpContext.Current != null &&
        //            HttpContext.Current.Request != null &&
        //            HttpContext.Current.Request.Url != null &&
        //            HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("/admin"))
        //        {
        //            authType = "AdminCookie";
        //        }
        //    }
        //    catch
        //    {
        //        // Bỏ qua lỗi, giữ nguyên CustomerCookie
        //    }

        //    return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager, authType);
        //}


        //public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        //{
        //    return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager, "CustomerCookie");
        //}

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
