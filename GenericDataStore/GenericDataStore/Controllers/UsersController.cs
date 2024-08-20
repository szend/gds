using GenericDataStore.DatabaseConnector;
using GenericDataStore.InputModels;
using GenericDataStore.Models;
using GenericDataStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GenericDataStore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;

        private readonly JwtService _jwtService;
        protected readonly RoleManager<AppRole> _roleManager;
        protected ApplicationDbContext DbContext { get; set; }



        public UsersController(
          UserManager<AppUser> userManager, JwtService jwtService, RoleManager<AppRole> roleManager, ApplicationDbContext dbContext,SignInManager<AppUser> signInManager
        )
        {
            _userManager = userManager;
            _signinManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            this.DbContext = dbContext;

        }

        [HttpPost]
        public async Task<ActionResult<AppUser>> PostUser(InputUser user)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<string> errors = new List<string>();
            if (!PasswordCheckUpperCase(user.Password))
            {
                errors.Add("Password must contain at least one uppercase letter");
            }
            if(!PasswordCheckLowerCase(user.Password))
            {
                errors.Add("Password must contain at least one lowercase letter");
            }
            if (!PasswordCheckDigit(user.Password))
            {
                errors.Add("Password must contain at least one digit");
            }
            if (!PasswordCheckNonAlphanumeric(user.Password))
            {
                errors.Add("Password must contain at least one non-alphanumeric character");
            }
            if (!PasswordCheckLength(user.Password))
            {
                errors.Add("Password must be at least 8 characters long");
            }
            if (errors.Count > 0)
            {
                return BadRequest(errors.Select(x => new {description = x}));
            }

            string psw = this.EncodePassword(user.Password);

            var result = await _userManager.CreateAsync(
                new AppUser() { UserName = user.Name, Email = user.Email, PasswordHash = psw, AllowedListCount = 2, AllowedDataCount = 50, AllowedExternalDataCount = 2000 });

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            user.Password = "";
            return Created("", user);
        }

        private bool PasswordCheckUpperCase(string password)
        {
            return password.Any(char.IsUpper);
        }
        private bool PasswordCheckLowerCase(string password)
        {
            return password.Any(char.IsLower);
        }

        private bool PasswordCheckDigit(string password)
        {
            return password.Any(char.IsDigit);
        }

        private bool PasswordCheckNonAlphanumeric(string password)
        {
            return password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        private bool PasswordCheckLength(string password)
        {
            return password.Length >= 8;
        }

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


        [HttpGet("{username}")]
        public async Task<ActionResult<AppUser>> GetUser(string username)
        {
            AppUser user = await _userManager.FindByEmailAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return new AppUser
            {
                UserName = user.UserName,
                Id = user.Id
            };
        }

        [HttpGet("UserInRole/{role}/{name}")]
        public async Task<ActionResult> UserInRole(string role, string name)
        {
            bool res = UserRoleCheck(role, name).Result;
            return new JsonResult(res, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("BearerToken")]
        public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken(InputUser request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad credentials");
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return BadRequest("Bad credentials");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }
            var v = User.Identity.Name;

            var token = _jwtService.CreateToken(user);

            return Ok(token);
        }

        [Authorize(Policy = "Jwt")]
        [HttpDelete("DeleteUser/{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            if (!UserRoleCheck("Administrators", User.Identity.Name).Result)
            {
                return BadRequest();
            }

            AppUser user = await _userManager.FindByNameAsync(username);
            await _userManager.DeleteAsync(user);
            return Ok();
        }

        [Authorize(Policy = "Jwt")]
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult> GetAllUsers()
        {
            if (!UserRoleCheck("Administrators", User.Identity.Name).Result)
            {
                return BadRequest();
            }

            var users = _userManager.Users.ToList().Select(x => new { name = x.UserName, roles = _userManager.GetRolesAsync(x).Result.ToList() }).ToList();

            return new JsonResult(users, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [Authorize(Policy = "Full")]
        [HttpGet("AddRoleToUser/{role}/{name}")]
        public async Task<ActionResult> AddRoleToUser(string role, string name)
        {
            if (!UserRoleCheck("Administrators", User.Identity.Name).Result)
            {
                return BadRequest(false);
            }

            var user = await _userManager.FindByNameAsync(name);

            if (user == null)
            {
                return BadRequest(false);
            }

            if (!await _roleManager.Roles.AnyAsync(e => e.NormalizedName == role.ToUpper()))
            {
                await _roleManager.CreateAsync(new AppRole() { Name = role });
            }

            await this._userManager.AddToRoleAsync(user, role);

            return Ok(true);
        }

        [Authorize(Policy = "Full")]
        [HttpGet("RemoveRoleFromUser/{role}/{name}")]
        public async Task<ActionResult> RemoveRoleFromUser(string role, string name)
        {
            if (!UserRoleCheck("Administrators", User.Identity.Name).Result)
            {
                return BadRequest(false);
            }

            var user = await _userManager.FindByNameAsync(name);

            if (user == null)
            {
                return BadRequest(false);
            }


            await this._userManager.RemoveFromRoleAsync(user, role);

            return Ok(true);
        }

        [Authorize(Policy = "Full")]
        [HttpGet("Logout")]
        public async Task<ActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return Ok(true);
        }

        private async Task<bool> UserRoleCheck(string role, string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            var userRoles = await _userManager.GetRolesAsync(user);
            bool res = userRoles.Contains(role);
            return Task.FromResult(res).Result;
        }

        [Authorize(Policy = "Full")]
        [HttpGet("ListOwner/{id}")]
        public async Task<ActionResult> ListOwner(Guid id)
        {
            var user = await this._userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return Ok(false);
            }
            var list = await this.DbContext.ObjectType.FirstOrDefaultAsync(x => x.ObjectTypeId == id);
            return Ok(list.AppUserId == user.Id);
        }

        [Authorize(Policy = "Full")]
        [HttpGet("GetSettingData/")]
        public async Task<ActionResult> GetSettingData()
        {
            var user = await this._userManager.FindByNameAsync(User.Identity.Name);
            var alllist = DbContext.ObjectType.Where(x => x.AppUserId == user.Id).ToList();
            var alldatacount = 0;
            var allexternaldatacount = 0;
            var dbids = DbContext.DatabaseConnectionProperty.Where(x => x.AppUserId == user.Id).ToList();
            foreach (var item in dbids)
            {
                var repo = new Repository(item.ConnectionString, item.DatabaseType);
                var lists = DbContext.ObjectType.Where(x => x.DatabaseConnectionPropertyId == item.DatabaseConnectionPropertyId).ToList();
                foreach (var item2 in lists)
                {
                    allexternaldatacount += repo.GetCount(item2);

                }

            }



            var internaldatabases = DbContext.DatabaseConnectionProperty.Where(x => x.Default == true).ToList();
            foreach (var item in internaldatabases)
            {
                var repo = new Repository(item.ConnectionString, item.DatabaseType);
                var lists = DbContext.ObjectType.Where(x => x.DatabaseConnectionPropertyId == item.DatabaseConnectionPropertyId && x.AppUserId == user.Id).ToList();
                foreach (var item2 in lists)
                {
                    alldatacount += repo.GetCount(item2);

                }
            }
            return new JsonResult(new { maxlist = user.AllowedListCount, maxdata = user.AllowedDataCount, currentlist = alllist.Count(), currentdata = alldatacount, maxexterndata = user.AllowedExternalDataCount, currentexdata = allexternaldatacount }, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }



        [HttpGet("CheckSub/{sec}")]
        public async Task<ActionResult> CheckSub(string sec)
        {
            var users = this._userManager.Users.ToList();
            int payed = 0;
            foreach (var item in users)
            {
                if(item.HasSub == true)
                {
                    var alldatacount = 0;
                    var allexternaldatacount = 0;
                    var dbids = DbContext.DatabaseConnectionProperty.Where(x => x.AppUserId == item.Id).ToList();
                    foreach (var item2 in dbids)
                    {
                        var repo = new Repository(item2.ConnectionString, item2.DatabaseType);
                        var lists = DbContext.ObjectType.Where(x => x.DatabaseConnectionPropertyId == item2.DatabaseConnectionPropertyId).ToList();
                        foreach (var item3 in lists)
                        {
                            allexternaldatacount += repo.GetCount(item3);

                        }

                    }

                    var internaldatabases = DbContext.DatabaseConnectionProperty.Where(x => x.Default == true).ToList();
                    foreach (var item2 in internaldatabases)
                    {
                        var repo = new Repository(item2.ConnectionString, item2.DatabaseType);
                        var lists = DbContext.ObjectType.Where(x => x.DatabaseConnectionPropertyId == item2.DatabaseConnectionPropertyId && x.AppUserId == item.Id).ToList();
                        foreach (var item3 in lists)
                        {
                            alldatacount += repo.GetCount(item3);

                        }
                    }

                    
                    if (item.NextPay < DateTime.Now)
                    {
                        item.MaxDataCountInMonth = alldatacount;
                        item.MaxExternalDataCountInMonth = allexternaldatacount;
                        item.NextPay = DateTime.Now.AddMonths(1);
                        payed++;
                    }
                    else
                    {
                        if (item.MaxDataCountInMonth < alldatacount)
                        { 
                            item.MaxDataCountInMonth = alldatacount;
                        }
                        if (item.MaxExternalDataCountInMonth < allexternaldatacount)
                        {
                            item.MaxExternalDataCountInMonth = allexternaldatacount;
                        }
                    }
                    await _userManager.UpdateAsync(item);

                }
            }

            return new JsonResult(new {payeduser = payed }, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }

        [Authorize(Policy = "Full")]
        [HttpGet("SetLimit/{datacount}/{extdatacount}/{listcount}/{src}")]
        public async Task<ActionResult> SetLimit(int datacount, int extdatacount, int listcount, string src)
        {
            var user = await this._userManager.FindByNameAsync(User.Identity.Name);

            if(src == "xxy93")
            {
                user.AllowedDataCount = datacount > 0 ? datacount : user.AllowedDataCount;
                user.AllowedExternalDataCount = extdatacount > 0 ? extdatacount : user.AllowedExternalDataCount;
                user.AllowedListCount = listcount > 0 ? listcount : user.AllowedListCount;
                user.HasSub = true;
                await _userManager.UpdateAsync(user);
            }



            return Ok();

        }

    }
}
