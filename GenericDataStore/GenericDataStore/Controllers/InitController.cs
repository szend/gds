using GenericDataStore.DatabaseConnector;
using GenericDataStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text.Json;

namespace GenericDataStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InitController : ControllerBase
    {
        public InitController(ILogger<InitController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.userManager = userManager;
        }

        private readonly UserManager<AppUser> userManager;
        protected ILogger<InitController> Logger { get; set; }

        protected ApplicationDbContext DbContext { get; set; }

        private string EncodePassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        [HttpGet("migr/{psw}/{sec}")]
        public async Task<ContentResult> Migr(string? psw = "´", string? sec = " ", string? email = "admin@admin.admin")
        {
            try
            {
                if (sec != "grtkkj6.A")
                {
                    return new ContentResult() { };
                }
                var migrations = await DbContext.Database.GetPendingMigrationsAsync();
                string mess = "";
                if (migrations.Any())
                {
                    Logger.LogWarning("InitDatabase.Migrate");
                    await DbContext.Database.MigrateAsync();
                    mess += "Migration done. ";
                }



                if (psw != "" && !await this.userManager.Users.AnyAsync(e => e.NormalizedUserName == "ADMIN"))
                {
                    psw = this.EncodePassword(psw);
                    AppUser admin = new AppUser() { UserName = "admin", AllowedDataCount = 999999999, AllowedListCount = 999999999, AllowedExternalDataCount = 999999999, PasswordHash = psw, Email = email };
                    var result = await this.userManager.CreateAsync(admin);

                    if (!result.Succeeded)
                    {
                        return new ContentResult() { Content = mess };
                    }
                    DbContext.SaveChanges();
                    mess += "Admin user created. ";
                }

                return new ContentResult() { Content = mess };

            }
            catch (Exception e)
            {

                return new ContentResult() { Content = e.Message +".........."+
                    e.Source + "........." +
                    e.StackTrace
                
            };
            }


        }

    }
}
